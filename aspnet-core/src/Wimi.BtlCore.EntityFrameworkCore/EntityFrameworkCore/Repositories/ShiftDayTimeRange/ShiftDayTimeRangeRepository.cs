using Abp.Application.Services.Dto;

namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.ShiftDayTimeRange
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Configuration;
    using Dapper;
    using Wimi.BtlCore.Configuration;
    using Wimi.BtlCore.ShiftDayTimeRange;

    public class ShiftDayTimeRangeRepository : IShiftDayTimeRangeRepository
    {
        private readonly string connectionString;
        private readonly ISettingManager settingManager;

        public ShiftDayTimeRangeRepository(ISettingManager settingManager)
        {
            this.settingManager = settingManager;

            connectionString = AppSettings.Database.ConnectionString;//this.settingManager.GetSettingValue(AppSettings.Database.ConnectionString);
        }

        public async Task<IEnumerable<ShiftDayTimeRange>> ListShiftDayTimeRanges(IEnumerable<int> machineIds, DateTime startTime, DateTime endTime)
        {
            var sql = @" SELECT m.Id AS MachineId, ssi.Id AS OrderSeq,
                                CONVERT(NVARCHAR, msd.ShiftDay, 23) AS ShiftDay,
                                CONVERT(NVARCHAR, ssi.StartTime, 108) AS BeginTime,
                                CONVERT(NVARCHAR, ssi.EndTime, 108) AS FinishTime
                        FROM   Machines  AS m
                                INNER JOIN MachinesShiftDetails AS msd ON  m.Id = msd.MachineId
                                INNER JOIN ShiftSolutionItems AS ssi ON  msd.ShiftSolutionItemId = ssi.Id
                        WHERE  m.Id IN @MachineIds AND msd.ShiftDay BETWEEN @StartTime AND @EndTime 
                        ORDER BY msd.MachineId, msd.ShiftDay, ssi.Id  ";

            using (var conn = new SqlConnection(this.connectionString))
            {
                var query = await conn.QueryAsync<ShiftDayTimeRange>(sql, new { machineIds, startTime, endTime });
                var result = query
                    .GroupBy(
                        q => new { q.MachineId, q.ShiftDay },
                        (key, g) => new
                        {
                            Item = key,
                            StartTime = Convert.ToDateTime($"{key.ShiftDay:yyyy-MM-dd} {g.First().BeginTime}"),
                            EndTime = Convert.ToDateTime($"{key.ShiftDay:yyyy-MM-dd} {g.Last().FinishTime}")
                        }).Select(
                        r => new ShiftDayTimeRange
                        {
                            MachineId = r.Item.MachineId,
                            ShiftDay = r.Item.ShiftDay,
                            StartTime = r.StartTime,
                            EndTime = r.StartTime >= r.EndTime ? r.EndTime.AddDays(1) : r.EndTime
                        }).ToList();

                return result;
            }
        }


        public async Task<IEnumerable<ShiftDayTimeRange>> ListMachineShiftDayTimeRange(int machineId, DateTime shiftDay)
        {
            const string sql = @"SELECT t.MachineId,
                                       t.MachineShiftDetailId,
                                       t.ShiftDay,
                                       t.BeginDay + ' ' + t.BeginTime   AS BeginTime,
                                       t.FinishDay + ' ' + t.FinishTime  AS FinishTime
                                FROM   (
                                           SELECT m.Id      AS MachineId,
                                                  msd.Id    AS MachineShiftDetailId,
                                                  CONVERT(NVARCHAR, msd.ShiftDay, 23) AS ShiftDay,
                                                  CASE 
                                                       WHEN ssi.IsNextDay = 1  AND CONVERT(NVARCHAR, ssi.StartTime, 108) <= CONVERT(NVARCHAR, ssi.EndTime, 108) THEN CONVERT(NVARCHAR, DATEADD(DAY, 1, msd.ShiftDay), 23)
                                                       ELSE  CONVERT(NVARCHAR, msd.ShiftDay, 23)
                                                  END          BeginDay,
                                                  CASE 
                                                       WHEN ssi.IsNextDay = 0 AND CONVERT(NVARCHAR, ssi.StartTime, 108) <= CONVERT(NVARCHAR, ssi.EndTime, 108) THEN CONVERT(NVARCHAR, msd.ShiftDay, 23)
                                                       ELSE CONVERT(NVARCHAR, DATEADD(DAY, 1, msd.ShiftDay), 23)
                                                  END          FinishDay,
                                                  CONVERT(NVARCHAR, ssi.StartTime, 108) AS BeginTime,
                                                  CONVERT(NVARCHAR, ssi.EndTime, 108) AS FinishTime
                                           FROM   Machines  AS m
                                                  INNER JOIN MachinesShiftDetails AS msd ON  m.Id = msd.MachineId
                                                  INNER JOIN ShiftSolutionItems AS ssi ON  msd.ShiftSolutionItemId = ssi.Id
                                           WHERE  m.Id = @machineId AND msd.ShiftDay = @shiftDay
                                       )  AS t";
            using (var conn = new SqlConnection(this.connectionString))
            {
                return await conn.QueryAsync<ShiftDayTimeRange>(sql, new { machineId, shiftDay });
            }
        }

        public async Task<IEnumerable<NameValueDto<int>>> ListSummaryDataRange(int startDateKey, int endDateKey)
        {
            const string sql = "SELECT c.DateKey AS Value,CONVERT(NVARCHAR, c.[Date],23) AS Name FROM Calendars AS c WHERE c.[DateKey] BETWEEN @startDateKey AND @endDateKey  ORDER BY c.DateKey DESC  ";
            using (var conn = new SqlConnection(this.connectionString))
            {
                return await conn.QueryAsync<NameValueDto<int>>(sql, new { startDateKey, endDateKey });
            }
        }

        public IEnumerable<ShiftDayTimeRange> ListMachineShiftDayTimeRange()
        {
            const string sql = @"select 
	                                   scv.MachineId,
	                                   scv.ShiftDay,
	                                   scv.ShiftSolutionId as ShiftSolutionId ,
	                                   scv.ShiftSolutionName,
	                                   scv.ShiftItemId as ShiftSolutionItemId,
	                                   scv.ShiftItemName, 
	                                   scv.MachineShiftDetailId as MachineShiftDetailId,
	                                   scv.StartTime as begintime,
	                                   scv.EndTime as finishtime
                                  from ShiftCalendarsView AS scv where getdate() BETWEEN scv.StartTime and scv.EndTime order by scv.StartTime";
            using (var conn = new SqlConnection(this.connectionString))
            {
                var result = conn.Query<ShiftDayTimeRange>(sql);
                return result;
            }
        }

        public IEnumerable<ShiftDayTimeRange> ListMachineShiftDayTimeRange(IEnumerable<int> machineIds)
        {
            const string sql = @"select 
	                                   scv.MachineId,
	                                   scv.ShiftDay,
									   scv.ShiftSolutionId,
									   scv.ShiftSolutionName,
	                                   min(scv.StartTime) as StartTime,
	                                   max(scv.EndTime) as EndTime
                                  from ShiftCalendarsView AS scv 
								  where ShiftDay in 
								  (
								    select distinct ShiftDay
									from ShiftCalendarsView
									where getdate() BETWEEN StartTime and EndTime
									and MachineId in @MachineIds
								  )
								  and MachineId in @MachineIds
								  group by scv.MachineId,scv.ShiftDay,scv.ShiftSolutionId,scv.ShiftSolutionName";
            using (var conn = new SqlConnection(this.connectionString))
            {
                var result = conn.Query<ShiftDayTimeRange>(sql, new { MachineIds = machineIds });
                return result;
            }
        }



        public string GetShiftItemName(int machineShiftDetailId)
        {
            using (var conn = new SqlConnection(this.connectionString))
            {
                const string sql = "select ShiftItemName from  ShiftCalendarsView  where MachineShiftDetailId = @machineShiftDetailId";
                return conn.QueryFirstOrDefault<string>(sql, new { machineShiftDetailId });
            }
        }

        public IEnumerable<ShiftEffectiveIntervalTimeRange> ListMachineShiftEffectiveIntervalTimeRange(int shiftSolutionId)
        {
            const string sql = @"WITH ShiftEffectiveInterval AS(
                                     SELECT msei.Id,msei.ShiftSolutionId,msei.MachineId,msei.StartTime,msei.EndTime,
                                           ((CASE 
                                                  WHEN si.IsNextDay=0 THEN DATEADD(DAY,datediff(DAY,si.StartTime,CONVERT(NVARCHAR(10), msei.StartTime, 23)),si.StartTime) 
					                              WHEN si.IsNextDay=1 AND  DATEDIFF(MINUTE,DATEADD(DAY,datediff(DAY,si.StartTime,CONVERT(NVARCHAR(10), msei.StartTime, 23))+1,si.EndTime),DATEADD(DAY,datediff(DAY,si.StartTime,CONVERT(NVARCHAR(10), msei.StartTime, 23))+1,si.StartTime))<0 
					                              THEN DATEADD(DAY,datediff(DAY,si.StartTime,CONVERT(NVARCHAR(10), msei.StartTime, 23))+1,si.StartTime)
					                              ELSE DATEADD(DAY,datediff(DAY,si.StartTime,CONVERT(NVARCHAR(10), msei.StartTime, 23)),si.StartTime)  
                                             END))    ShiftStartTime,
				                           (CASE 
                                                 WHEN si.IsNextDay=1 
                                                 THEN DATEADD(DAY,datediff(DAY,si.StartTime,CONVERT(NVARCHAR(10), msei.EndTime, 23))+1,si.EndTime) 
                                                 ELSE DATEADD(DAY,datediff(DAY,si.StartTime,CONVERT(NVARCHAR(10), msei.EndTime, 23)),si.EndTime)
                                            END) ShiftEndTime                    
                                     FROM MachineShiftEffectiveIntervals AS msei
                                     INNER JOIN ShiftSolutionItems AS si ON msei.ShiftSolutionId = si.ShiftSolutionId  
                                     WHERE msei.IsDeleted=0
                                     AND msei.ShiftSolutionId=@ShiftSolutionId 
                                    )

                                 SELECT sei.Id ShiftEffectiveIntervalId, MIN(sei.ShiftStartTime) StartTime , MAX(sei.ShiftEndTime) EndTime
                                 FROM ShiftEffectiveInterval AS sei
                                 GROUP BY sei.Id";
            using (var conn = new SqlConnection(this.connectionString))
            {
                var result = conn.Query<ShiftEffectiveIntervalTimeRange>(sql, new { ShiftSolutionId = shiftSolutionId });
                return result;
            }

        }

        public MachineCapacityShiftDetail GetMachineCurrentShiftDetail(long shiftDetailId)
        {
            const string sql = @"SELECT msd.Id                        AS MachineShiftDetailId,
                                       ss.Name                        AS ShiftDetail_SolutionName,
                                       ssi.Name                       AS ShiftDetail_MachineShiftName,
                                       msd.ShiftDay                   AS ShiftDetail_ShiftDay
                                FROM   MachinesShiftDetails           AS msd
                                       INNER JOIN ShiftSolutions      AS ss ON  msd.ShiftSolutionId = ss.Id
                                       INNER JOIN ShiftSolutionItems  AS ssi ON  msd.ShiftSolutionItemId = ssi.Id
                                WHERE  msd.id = @MachinesShiftDetailId";

            using (var conn = new SqlConnection(this.connectionString))
            {
                var value = conn.QueryFirstOrDefault<MachineCapacityShiftDetail>(sql, new { MachinesShiftDetailId = shiftDetailId });
                return value ?? new MachineCapacityShiftDetail();
            }
        }
        public ShiftDayTimeRange GetMachineNaturalDayShift(DateTime day, int machineId)
        {
            const string sql = @"SELECT sc.MachineShiftDetailId       AS MachineShiftDetailId,  
                                        sc.MachineId                  AS MachineId,
                                        sc.ShiftDay
                                FROM   ShiftCalendarsView             AS sc
                                WHERE  sc.MachineId= @MachineId And StartTime = @StartTime";

            using (var conn = new SqlConnection(this.connectionString))
            {
                var value = conn.QueryFirstOrDefault<ShiftDayTimeRange>(sql, new { MachineId = machineId, StartTime = day });
                return value;
            }
        }

        public ShiftDayTimeRange ListMachineShiftDayTimeRange(int machineShiftDetailId)
        {
            const string sql = @"select 
                                       scv.MachineShiftDetailId,
	                                   scv.MachineId,
	                                   scv.ShiftDay,
									   scv.ShiftSolutionId,
									   scv.ShiftSolutionName,
	                                   min(scv.StartTime) as StartTime,
	                                   max(scv.EndTime) as EndTime
                                  from ShiftCalendarsView AS scv 
								  where  scv.MachineShiftDetailId = @machineShiftDetailId
								  group by scv.MachineId,scv.ShiftDay,scv.MachineShiftDetailId,scv.ShiftSolutionId,scv.ShiftSolutionName";
            using (var conn = new SqlConnection(this.connectionString))
            {
                var result = conn.QueryFirstOrDefault<ShiftDayTimeRange>(sql, new { machineShiftDetailId });
                return result;
            }
        }
    }
}