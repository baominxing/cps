using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Shift;
using Wimi.BtlCore.Shift.Dtos;

namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.Shift
{
    public class ShiftRepository : IShiftRepository
    {
        public async Task BatchDeleteMachineShift(List<int> machineIdList)
        {
            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlEffectiveDelete = "update MachineShiftEffectiveIntervals set [IsDeleted] = 1 where StartTime > @today and machineId in @machineIds";
                        await conn.ExecuteAsync(sqlEffectiveDelete, new { today = DateTime.Today, machineIds = machineIdList.ToArray() }, tran);

                        string sqlEffectiveUpdate = "update MachineShiftEffectiveIntervals set EndTime = @today where machineId in @machineIds  AND  EndTime > @today AND StartTime <= @today AND IsDeleted = 0 ";
                        await conn.ExecuteAsync(sqlEffectiveUpdate, new { today = DateTime.Today, machineIds = machineIdList.ToArray() }, tran);

                        for (int i = 0; i < machineIdList.Count; i++)
                        {
                            var item = machineIdList[i];
                            string sql = "delete from MachinesShiftDetails where shiftday > @today and machineId = @machineId";
                            await conn.ExecuteAsync(sql, new { today = DateTime.Today, machineId = item }, tran);

                            string sql2 = "delete from ShiftCalendars where shiftday > @today and machineId = @machineId";
                            await conn.ExecuteAsync(sql2, new { today = DateTime.Today, machineId = item }, tran);
                        }
                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> CheckIfCurrentDayShiftIsWorking(int machineId, int? id)
        {
            var flag = false;

            var executeSql = Sql.CheckIfCurrentDayShiftIsWorkingSql;

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();
                var sqlParameters = new { machineId, id };
                var count = (await conn.QueryAsync(executeSql, sqlParameters)).Count();

                if (count > 0) flag = true;
            }

            return flag;
        }

        public async Task<IEnumerable<DeviceHistoryShiftInfoDto>> GetDeviceHistoryShiftInfo(int deviceId)
        {
            if (deviceId == 0) return new List<DeviceHistoryShiftInfoDto>();

            var executeSql = @"
                SELECT sm.[ShiftSolutionId]
                      ,sm.[MachineId] DeviceId
                      ,Convert(char(10),sm.StartTime,23) StartDate
                      ,Convert(char(10),sm.[EndTime],23) EndDate 
                      ,ss.Name ShiftSolutionName
                      ,si.Name ShiftName
                      ,Convert(char(8),si.StartTime,24) StartTime 
                      ,Convert(char(8), si.EndTime,24) EndTime 
                      ,si.Duration
                      ,CASE WHEN GETDATE() >= CONVERT(NVARCHAR(10), sm.StartTime, 23) + ' ' + CONVERT(VARCHAR(8), si.StartTime, 108) THEN 1
                      ELSE 0 END IsHistory
                  FROM MachineShiftEffectiveIntervals sm 
                  JOIN ShiftSolutions AS ss ON sm.ShiftSolutionId = ss.Id
                  JOIN ShiftSolutionItems si ON sm.ShiftSolutionId = si.ShiftSolutionId
                WHERE sm.MachineId = @MachineId AND sm.StartTime<=GETDATE() AND sm.IsDeleted = 0
                ORDER BY sm.StartTime Desc
                ";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();
                var sqlParameters = new { MachineId = deviceId };

                var query = await conn.QueryAsync<DeviceHistoryShiftInfoResult>(executeSql, sqlParameters);

                var queryDtos = query.ToList();

                var result = new List<DeviceHistoryShiftInfoDto>();

                for (var i = 0; i < queryDtos.Count(); i++)
                {
                    if (result.Any(s => s.StartDate == queryDtos[i].StartDate))
                    {
                        var deviceHistoryShiftInfoDto =
                            result.FirstOrDefault(s => s.StartDate == queryDtos[i].StartDate);

                        deviceHistoryShiftInfoDto?.ShiftInfoDtos.Add(
                            new ShiftInfoDto2
                            {
                                Name = queryDtos[i].ShiftName,
                                StartTime = queryDtos[i].StartTime,
                                EndTime = queryDtos[i].EndTime,
                                Duration = queryDtos[i].Duration
                            });
                    }
                    else
                    {
                        var notStartToday = queryDtos[i].StartDate != DateTime.Now.ToString("yyyy-MM-dd");
                        bool isHistory = queryDtos[i].IsHistory && notStartToday;
                        result.Add(
                            new DeviceHistoryShiftInfoDto
                            {
                                DeviceId = queryDtos[i].DeviceId,
                                StartDate = queryDtos[i].StartDate,
                                EndDate = queryDtos[i].EndDate,
                                ShiftSolutionName = queryDtos[i].ShiftSolutionName,
                                IsHistory = isHistory,
                                ShiftInfoDtos =
                                        new List<ShiftInfoDto2>
                                            {
                                                new ShiftInfoDto2
                                                    {
                                                        Name = queryDtos[i].ShiftName,
                                                        StartTime = queryDtos[i].StartTime,
                                                        EndTime = queryDtos[i].EndTime,
                                                        Duration = queryDtos[i].Duration
                                                    }
                                            }
                            });
                    }
                }

                return result;
            }
        }

        public async Task<IEnumerable<MachineShiftSolutionDto>> GetMachineShiftSolutions(string ids, int? shiftSolutionId, int start, int length, string queryType)
        {
            if (string.IsNullOrEmpty(ids)) return new List<MachineShiftSolutionDto>();

            string whereClause;
            switch (queryType)
            {
                case "0":
                    whereClause = "where m.Id IN (Select item from dbo.func_SplitInts(@Ids, ','))";
                    break;
                default:
                    whereClause = "where dg.Id IN (Select item from dbo.func_SplitInts(@Ids, ','))";
                    break;
            }

            if (shiftSolutionId != null && shiftSolutionId != 0)
                whereClause += " AND (sm.ShiftSolutionId = @ShiftSolutionId)";

            var executeSql = Sql.GetMachineShiftSolutionsSql(whereClause);

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();
                var sqlParameters = new
                {
                    ids,
                    shiftSolutionId,
                    PageStart = start,
                    PageEnd = length + start
                };

                var queryDtos = await conn.QueryAsync<MachineShiftSolutionDto>(executeSql, sqlParameters);
                var dics = new Dictionary<int, MachineShiftSolutionDto>();
                var machineShiftSolutionDtos = queryDtos.ToList();
                foreach (var ms in machineShiftSolutionDtos)
                {
                    var startTime = Convert.ToDateTime(ms.StartTime);
                    if (dics.ContainsKey(ms.MachineId))
                    {

                        if (Convert.ToDateTime(dics[ms.MachineId].StartTime) >= startTime)
                        {
                            dics[ms.MachineId] = ms;
                        }
                    }
                    else
                    {
                        dics[ms.MachineId] = ms;
                    }

                }
                return dics.Values;
            }
        }
 
        private static class Sql
        {
            public static readonly string CheckIfCurrentDayShiftIsWorkingSql = @"
                    SELECT msei.Id
FROM   MachineShiftEffectiveIntervals  AS msei
       JOIN ShiftSolutionItems        AS si
            ON  msei.ShiftSolutionId = si.ShiftSolutionId
WHERE  msei.MachineId = @MachineId
       AND msei.Id = @Id
       AND GETDATE() >= CONVERT(NVARCHAR(10), msei.StartTime, 23) + ' ' + 
           CONVERT(VARCHAR(8), si.StartTime, 108)
                    ";

            public static string GetMachineShiftSolutionsSql(string whereClause)
            {
                return
                    $@"
SELECT A.Id,
    B.MachineGroupName,
    A.MachineId,
    A.MachineName,
    A.ShiftSolutionId,
    A.ShiftSolutionName,
    A.CreationTime,
    A.StartTime,
    A.EndTime
FROM   (
           SELECT CASE 
                       WHEN sm.isdeleted = 1 THEN 0
                       ELSE sm.Id
                  END        Id,
                  m.Id       MachineId,
                  m.Name     MachineName,
                  m.SortSeq  MachineSortSeq,
				  m.Code     MachineCode,
                  CASE 
                       WHEN sm.isdeleted = 1 THEN 0
                       ELSE ISNULL(sm.ShiftSolutionId, 0)
                  END        ShiftSolutionId,
                  CASE 
                       WHEN sm.isdeleted = 1 THEN 'NotAssociated'
                       ELSE ISNULL(ss.Name, 'NotAssociated')
                  END ShiftSolutionName,
                  CASE 
                       WHEN sm.isdeleted = 1 THEN NULL
                       ELSE CONVERT(CHAR(10), ISNULL(sm.CreationTime, NULL), 23)
                  END CreationTime,
                  StartTime,
                  EndTime
           FROM   DeviceGroups AS dg
                  INNER JOIN MachineDeviceGroups AS mdg ON dg.Id = mdg.DeviceGroupId
                  INNER JOIN Machines AS m  ON  mdg.MachineId = m.Id
                  LEFT JOIN MachineShiftEffectiveIntervals AS sm
                       ON  m.Id = sm.MachineId
                       AND sm.EndTime>=convert(date,GETDATE()) 
                       AND sm.IsDeleted = 0
                  LEFT JOIN ShiftSolutions AS ss
                       ON  sm.ShiftSolutionId = ss.Id
           { whereClause }
       ) AS A
       JOIN (
                SELECT m2.Id MachineId,
                       MachineGroupName = STUFF(
                           (
                               SELECT ',' + dg.DisplayName
                               FROM   Machines AS m
                                      INNER JOIN MachineDeviceGroups AS mdg
                                           ON  mdg.MachineId = m.Id
                                      JOIN DeviceGroups AS dg
                                           ON  dg.Id = mdg.DeviceGroupId
                               WHERE  m.Id = m2.Id AND dg.IsDeleted = 0 
                                      FOR XML PATH('')
                           ),
                           1,
                           1,
                           ''
                       )
                FROM   Machines AS m2
            ) B
            ON  a.MachineId = b.MachineId
GROUP BY
       A.Id,
       B.MachineGroupName,
       A.MachineId,
       A.MachineName,
       a.ShiftSolutionId,
       a.ShiftSolutionName,
       a.CreationTime,
       a.StartTime,
       a.EndTime,
	   A.MachineSortSeq,
	   A.MachineCode
ORDER BY 
       A.MachineSortSeq,
       A.MachineCode,
       a.MachineId
";
            }
        }
    }
}
