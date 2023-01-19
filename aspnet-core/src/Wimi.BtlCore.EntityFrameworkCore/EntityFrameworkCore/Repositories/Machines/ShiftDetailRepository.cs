using Abp.Configuration;
using Abp.UI;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Machines.Repository;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.CommonEnums;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.ThirdpartyApis.Dto;

namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.Machines
{
    public class ShiftDetailRepository : IShiftDetailRepository
    {
        private readonly ISettingManager settingManager;

        private readonly string connectionString;

        public ShiftDetailRepository(
            ISettingManager settingManager
            )
        {
            this.settingManager = settingManager;

            this.connectionString = AppSettings.Database.ConnectionString;
        }

        public async Task InsertMachineShiftDetailsAsync(DeviceShiftSolutionInput input)
        {
            var sql = @"
                        INSERT INTO MachinesShiftDetails
                          (
                            MachineId,
                            ShiftDay,
                            ShiftSolutionId,
                            ShiftSolutionItemId,
                            CreationTime,
                            CreatorUserId
                          )
                        SELECT m.Id                AS MachineId,
                               c.[Date]            AS ShiftDay,
                               ssi.ShiftSolutionId,
                               ssi.Id              AS ShiftSolutionItemId,
                               GETDATE()           AS CreationTime,
                               @CreatorUserId
                        FROM   Calendars           AS c,
                               Machines            AS m,
                               ShiftSolutionItems  AS ssi
                        WHERE  c.[Date] BETWEEN @StartTime AND @EndTime AND m.Id=@MachineId AND m.IsDeleted = 0 AND ssi.ShiftSolutionId = @ShiftSolutionId
                        ";

            var param = new { input.CreatorUserId, input.StartTime, input.EndTime, MachineId = input.DeviceId, input.ShiftSolutionId };

            using (var conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                await conn.ExecuteAsync(sql, param);
            }
        }

        /// <summary>
        /// 新装班次日历记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task InsertMachineShiftCalendarAsync(DeviceShiftSolutionInput input)
        {
            var sql = @"
SET DATEFIRST 1;

INSERT INTO [ShiftCalendars]
(
    [ShiftSolutionId],
    [ShiftItemId],
    [ShiftItemSeq],
    [MachineShiftDetailId],
    [MachineId],
    [StartTime],
    [EndTime],
    [Duration],
    [ShiftDay],
    [ShiftDayName],
    [ShiftWeek],
    [ShiftWeekName],
    [ShiftMonth],
    [ShiftMonthName],
    [ShiftYear],
    [ShiftYearName],
    [LastModificationTime],
    [LastModifierUserId],
    [CreationTime],
    [CreatorUserId]
)
SELECT t2.ShiftSolutionId,
       t2.ShiftItemId,
       RANK() OVER (PARTITION BY t1.MachineId,
                                 t1.ShiftSolutionId,
                                 t2.ShiftDay
                    ORDER BY t2.ShiftItemId
                   ) ShiftItemSeq,
       t1.Id MachineShiftDetailId,
       t2.MachineId,
       t2.StartTime,
       t2.EndTime,
       DATEDIFF(SECOND, t2.StartTime, t2.EndTime) Duration,
       t2.ShiftDay,
       CONVERT(NVARCHAR(10), t2.ShiftDay, 120) + N'日' ShiftDayName,
       DATEPART(WEEK, t2.ShiftDay) ShiftWeek,
       t2.YYYYWeek + N'周' ShiftWeekName,
       DATEPART(MONTH, t2.ShiftDay) ShiftMonth,
       t2.YYYYMM + N'月' ShiftMonthName,
       DATEPART(YEAR, t2.ShiftDay) ShiftYear,
       CONVERT(NVARCHAR(10), DATEPART(YEAR, t2.ShiftDay)) + N'年' ShiftYearName,
       NULL,
       0,
       GETDATE(),
       @CreatorUserId
FROM dbo.MachinesShiftDetails AS t1
    JOIN
    (
        SELECT m.Id AS MachineId,
               c.[Date] AS ShiftDay,
               ssi.ShiftSolutionId,
               ssi.Id AS ShiftItemId,
               c.YYYYWeek,
			   c.YYYYMM,
               CASE WHEN CONVERT(NVARCHAR(10), ssi.StartTime, 108) <CONVERT(NVARCHAR(10), ssi.EndTime, 108) AND ssi.IsNextDay = 1  THEN 
                    DATEADD(DAY,1,CONVERT(NVARCHAR(10), c.[Date], 120)+' '+ CONVERT(NVARCHAR(10), ssi.StartTime, 108) ) 
                    ELSE     CONVERT(NVARCHAR(10), c.[Date], 120) + ' ' + CONVERT(NVARCHAR(10), ssi.StartTime, 108) END  StartTime,
               CASE
                   WHEN ssi.IsNextDay = 1 THEN
                       DATEADD(
                                  DAY,
                                  1,
                                  CONVERT(NVARCHAR(10), c.[Date], 120) + ' ' + CONVERT(NVARCHAR(10), ssi.EndTime, 108)
                              )
                   ELSE
                       CONVERT(NVARCHAR(10), c.[Date], 120) + ' ' + CONVERT(NVARCHAR(10), ssi.EndTime, 108)
               END EndTime
        FROM Calendars AS c,
             Machines AS m,
             ShiftSolutionItems AS ssi
        WHERE c.[Date]
              BETWEEN @StartTime AND @EndTime
              AND m.Id = @MachineId
              AND m.IsDeleted = 0
              AND ssi.ShiftSolutionId = @ShiftSolutionId
    ) AS t2
        ON t1.MachineId = t2.MachineId
           AND t1.ShiftDay = t2.ShiftDay
           AND t1.ShiftSolutionId = t2.ShiftSolutionId
           AND t1.ShiftSolutionItemId = t2.ShiftItemId
";

            var param = new { input.CreatorUserId, input.StartTime, input.EndTime, MachineId = input.DeviceId, input.ShiftSolutionId };

            using (var conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                await conn.ExecuteAsync(sql, param);
            }
        }

        /// <summary>
        /// 获取当前班次的具体信息，班次方案，名称，班次日
        /// </summary>
        public MachineShiftDetail GetCurrentShiftDay()
        {
            // 计算每个设备的
            var sql =
                @"SELECT T.RowNum,T.MachineId,T.MachineShiftDetailId,T.ShiftDay, T.ShiftSolutionId,T.ShiftSolutionItemId,T.ShiftSolutionItemName
                        FROM   (
                                   SELECT ROW_NUMBER() OVER(PARTITION BY msd.MachineId ORDER BY msd.Id) AS 
                                          rowNum,
                                          msd.Id                AS MachineShiftDetailId,
                                          msd.MachineId,
                                          msd.ShiftDay,
                                          msd.ShiftSolutionId,
                                          msd.ShiftSolutionItemId,
                                          ssi.Name                 ShiftSolutionItemName
                                   FROM   MachinesShiftDetails  AS msd
                                          INNER JOIN (
                                                   SELECT T.MachineId,
                                                          CASE 
                                                               WHEN ssi.IsNextDay =1 AND  CONVERT(TIME, ssi.EndTime) >= CONVERT(TIME, GETDATE()) THEN  CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))
                                                               ELSE CONVERT(DATE, GETDATE())
                                                          END AS ShiftDay
                                                   FROM   (
                                                              SELECT msei.ShiftSolutionId, msei.MachineId, MAX(ssi.Id) AS ShiftSolutionItemId
                                                              FROM   MachineShiftEffectiveIntervals AS  msei
                                                                     INNER JOIN ShiftSolutionItems AS ssi ON  msei.ShiftSolutionId = ssi.ShiftSolutionId
                                                              WHERE  msei.IsDeleted = 0  AND DATEADD(DAY, 0, GETDATE()) BETWEEN CONVERT(NVARCHAR(10), msei.StartTime, 23) + ' ' + CONVERT(VARCHAR(8), ssi.StartTime, 108)
                                                              AND CONVERT(NVARCHAR(10), msei.EndTime, 23)  + ' ' + CONVERT(VARCHAR(8), ssi.EndTime, 108)
                                                              GROUP BY msei.ShiftSolutionId, msei.MachineId
                                                          ) T
                                                          INNER JOIN ShiftSolutionItems AS ssi ON  ssi.Id = T.ShiftSolutionItemId
                                               )   AS M ON  (msd.ShiftDay = M.ShiftDay AND msd.machineId = M.machineId)
                                          INNER JOIN ShiftSolutionItems AS ssi ON  ssi.Id = msd.ShiftSolutionItemId
                                   WHERE  GETDATE() BETWEEN 
                                          CASE  WHEN ssi.IsNextDay = 1  AND ssi.StartTime <= ssi.EndTime  THEN DATEADD(DAY,1, CONVERT(NVARCHAR(10), M.ShiftDay, 23) +' ' + CONVERT(VARCHAR(8), ssi.StartTime, 108))
                                               ELSE CONVERT(NVARCHAR(10), M.ShiftDay, 23) + ' '+ CONVERT(VARCHAR(8), ssi.StartTime, 108)
                                          END
                                          AND CASE 
							                       WHEN  ssi.EndTime< ssi.StartTime  THEN DATEADD(DAY,1, CONVERT(NVARCHAR(10), M.ShiftDay, 23) +' ' + CONVERT(VARCHAR(8), ssi.EndTime, 108))
                                                   WHEN ssi.IsNextDay = 1 THEN DATEADD(DAY,1, CONVERT(NVARCHAR(10), M.ShiftDay, 23) +' ' + CONVERT(VARCHAR(8), ssi.EndTime, 108))
                                                   ELSE CONVERT(NVARCHAR(10), M.ShiftDay, 23) + ' '+ CONVERT(VARCHAR(8), ssi.EndTime, 108)
                                              END
                               ) T
                        WHERE  T.RowNum = 1";

            var checkSql = @"SELECT COUNT(*)
                                FROM   (
                                           SELECT CASE 
                                                       WHEN ssi.IsNextDay = 1  AND ssi.StartTime < = ssi.EndTime  THEN CONVERT(NVARCHAR, DATEADD(DAY, 1, msei.StartTime), 23) 
                                                            + ' ' + CONVERT(NVARCHAR, ssi.StartTime, 108)
                                                       ELSE CONVERT(NVARCHAR, msei.StartTime, 23) + ' ' + 
                                                            CONVERT(NVARCHAR, ssi.StartTime, 108)
                                                  END  AS StartTime,
                                                  CASE 
                                                       WHEN ssi.IsNextDay = 1 THEN CONVERT(NVARCHAR, DATEADD(DAY, 1, msei.EndTime), 23) + ' ' + CONVERT(NVARCHAR, ssi.EndTime, 108)
                                                       ELSE CONVERT(NVARCHAR, msei.EndTime, 23) + ' ' + CONVERT(NVARCHAR, ssi.EndTime, 108)
                                                  END  AS EndTime
                                           FROM   MachineShiftEffectiveIntervals AS msei
                                                  INNER JOIN ShiftSolutionItems AS ssi ON  ssi.ShiftSolutionId = msei.ShiftSolutionId
                                       ) AS T
                                WHERE  GETDATE() BETWEEN T.StartTime AND T.EndTime";
            using (var conn = new SqlConnection(this.connectionString))
            {
                var count = conn.QueryFirst<int>(checkSql);

                var result = conn.Query<MachineShiftDetail>(sql, new { Day = count > 0 ? 0 : -1 }).ToList();

                if (!result.Any())
                {
                    return new MachineShiftDetail() { ShiftDay = new DateTime(1900, 01, 01) };
                }

                return result.FirstOrDefault();
            }
        }

        public IEnumerable<ShiftCalendarDto> CorrectQueryDate(GetMachineStateRateInputDto input)
        {
            IEnumerable<ShiftCalendarDto> shiftCalendarList;
            string showColumn = string.Empty;
            string groupBy = string.Empty;
            string orderBy = string.Empty;

            switch (input.StatisticalWay)
            {
                case EnumStatisticalWays.ByShift:
                case EnumStatisticalWays.ByDay:
                    showColumn = "DATE ShiftDay";
                    orderBy = "Order By DATE";
                    break;
                case EnumStatisticalWays.ByWeek:
                    showColumn = "YYYYISOWeek ShiftWeek";
                    groupBy = "GROUP BY YYYYISOWeek";
                    orderBy = "Order By YYYYISOWeek";
                    break;
                case EnumStatisticalWays.ByMonth:
                    showColumn = "YYYYMM ShiftMonth";
                    groupBy = "GROUP BY YYYYMM";
                    orderBy = "Order By YYYYMM";
                    break;
                case EnumStatisticalWays.ByYear:
                    showColumn = "Year ShiftYear";
                    groupBy = "GROUP BY Year";
                    orderBy = "Order By Year";
                    break;
            }

            string executeSql =
                $@"SELECT {showColumn}  FROM calendars WHERE DATE >=@StartTime AND DATE <=@EndTime {groupBy} {orderBy}";

            IEnumerable<ShiftCalendarDto> correctedQueryDateList = new List<ShiftCalendarDto>();

            using (var conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                var sqlParameters = new { input.StartTime, input.EndTime };

                correctedQueryDateList = conn.Query<ShiftCalendarDto>(executeSql, sqlParameters);
            }

            shiftCalendarList = this.GetShiftCalendars(input, correctedQueryDateList, input.StatisticalWay);

            return shiftCalendarList;
        }

        private IEnumerable<ShiftCalendarDto> GetShiftCalendars(GetMachineStateRateInputDto input,IEnumerable<ShiftCalendarDto> correctedQueryDateList, EnumStatisticalWays statisticalWay)
        {
            IEnumerable<ShiftCalendarDto> shiftCalendarList = new List<ShiftCalendarDto>();

            if (!correctedQueryDateList.Any())
            {
                return shiftCalendarList;
            }

            var shiftIdColumn = @"'0'";
            var startTimeColumn = "MIN(A.StartTime)";
            var endTimeColumn = @"CASE WHEN MAX(A.EndTime) > GETDATE() THEN GETDATE() ELSE MAX(A.EndTime) END";
            var whereClause = string.Empty;
            var groupByColumn = $@"GROUP BY A.ShiftId,A.ShiftDay, A.ShiftWeek, A.ShiftMonth, A.ShiftYear";
            switch (statisticalWay)
            {
                case EnumStatisticalWays.ByShift:
                    shiftIdColumn = @"msd.Id";
                    startTimeColumn = "A.StartTime";
                    endTimeColumn = @"CASE WHEN A.EndTime > GETDATE() THEN GETDATE() ELSE a.EndTime END";
                    whereClause =
                        $"c.DATE >= '{correctedQueryDateList.FirstOrDefault().ShiftDay}' AND c.DATE <= '{correctedQueryDateList.LastOrDefault().ShiftDay}'";
                    groupByColumn = string.Empty;
                    break;
                case EnumStatisticalWays.ByDay:
                    whereClause =
                        $"c.DATE >= '{correctedQueryDateList.FirstOrDefault().ShiftDay}' AND c.DATE <= '{correctedQueryDateList.LastOrDefault().ShiftDay}'";
                    break;
                case EnumStatisticalWays.ByWeek:
                    whereClause =
                        $"c.YYYYISOWeek >= '{correctedQueryDateList.FirstOrDefault().ShiftWeek}' AND c.YYYYISOWeek <= '{correctedQueryDateList.LastOrDefault().ShiftWeek}'";
                    break;
                case EnumStatisticalWays.ByMonth:
                    whereClause =
                        $"c.YYYYMM >= '{correctedQueryDateList.FirstOrDefault().ShiftMonth}' AND c.YYYYMM <= '{correctedQueryDateList.LastOrDefault().ShiftMonth}'";
                    break;
                case EnumStatisticalWays.ByYear:
                    whereClause =
                        $"c.Year >= '{correctedQueryDateList.FirstOrDefault().ShiftYear}' AND c.Year <= '{correctedQueryDateList.LastOrDefault().ShiftYear}'";
                    break;
            }

            string executeSql = $@"
SELECT A.ShiftId,
       A.ShiftDay,
       A.ShiftWeek,
       A.ShiftMonth,
       A.ShiftYear,
       {startTimeColumn} StartTime,
       {endTimeColumn} EndTime
FROM   (
           SELECT DISTINCT
                  {shiftIdColumn}   ShiftId,
                  c.[Date]          ShiftDay,
                  c.YYYYISOWeek     ShiftWeek,
                  c.YYYYMM          ShiftMonth,
                  c.[Year]          ShiftYear,
                  CASE 
                       WHEN ssi.IsNextDay = 1
           AND ssi.StartTime < ssi.EndTime THEN DATEADD(
                   DAY,
                   1,
                   CONVERT(NVARCHAR(10), msd.ShiftDay, 120) + ' ' + CONVERT(NVARCHAR(10), ssi.StartTime, 108)
               )
               ELSE CONVERT(NVARCHAR(10), msd.ShiftDay, 120) + ' ' + CONVERT(NVARCHAR(10), ssi.StartTime, 108)
               END StartTime,
           CASE 
                WHEN ssi.IsNextDay = 1 THEN DATEADD(
                         DAY,
                         1,
                         CONVERT(NVARCHAR(10), msd.ShiftDay, 120) + ' ' +
                         CONVERT(NVARCHAR(10), ssi.EndTime, 108)
                     )
                ELSE CONVERT(NVARCHAR(10), msd.ShiftDay, 120) + ' ' + CONVERT(NVARCHAR(10), ssi.EndTime, 108)
           END EndTime
           FROM Calendars AS c
           JOIN MachinesShiftDetails AS msd
           ON msd.ShiftDay = c.[Date]
           JOIN ShiftSolutionItems AS ssi
           ON ssi.Id = msd.ShiftSolutionItemId
           WHERE {whereClause}
           --AND msd.MachineId IN @MachineIdList
           AND (
                   CASE 
                        WHEN ssi.IsNextDay = 1
                   AND ssi.StartTime < ssi.EndTime THEN DATEADD(
                           DAY,
                           1,
                           CONVERT(NVARCHAR(10), msd.ShiftDay, 120) + ' ' +
                           CONVERT(NVARCHAR(10), ssi.StartTime, 108)
                       )
                       ELSE CONVERT(NVARCHAR(10), msd.ShiftDay, 120) + ' ' +
                       CONVERT(NVARCHAR(10), ssi.StartTime, 108)
                       END
               ) <= GETDATE()
       )    AS A
{groupByColumn}
Order by {startTimeColumn}
";
            var parameters = new { input.MachineIdList };

            using (var conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                shiftCalendarList = conn.Query<ShiftCalendarDto>(executeSql, parameters);
            }

            return shiftCalendarList;
        }

        public void DeleteShiftDetailsAndCalender(int machineId, int shiftSolutonId, DateTime? shiftDayGt = null, DateTime? shiftDayLt = null, EnumEqType eqType = EnumEqType.None)
        {
            var shiftDayGtWhere = string.Empty;
            var shiftDayLtWhere = string.Empty;

            if (shiftDayGt != null)
            {
                if (eqType == EnumEqType.Gt || eqType == EnumEqType.Both)
                {
                    shiftDayGtWhere = "And ShiftDay >= @ShiftDayGt";
                }
                else
                {
                    shiftDayGtWhere = "And ShiftDay > @ShiftDayGt";
                }
            }

            if (shiftDayLt != null)
            {
                if (eqType == EnumEqType.Lt || eqType == EnumEqType.Both)
                {
                    shiftDayLtWhere = "And ShiftDay <= @ShiftDayLt";
                }
                else
                {
                    shiftDayLtWhere = "And ShiftDay < @ShiftDayLt";
                }
            }

            var shiftDetailSql = @$"DELETE MachinesShiftDetails
                        WHERE MachineId = @MachineId
                        AND ShiftSolutionId = @ShiftSolutionId
                        {shiftDayGtWhere} 
                        {shiftDayLtWhere}   
                    ";

            var shiftCalenderSql = @$"DELETE ShiftCalendars
                        WHERE MachineId = @MachineId
                        AND ShiftSolutionId = @ShiftSolutionId
                        {shiftDayGtWhere} 
                        {shiftDayLtWhere}   
                    ";

            var parameters = new { MachineId = machineId, ShiftSolutionId = shiftSolutonId, ShiftDayGt = shiftDayGt, ShiftDayLt = shiftDayLt };

            using (var conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                conn.Execute(shiftDetailSql, parameters);
                conn.Execute(shiftCalenderSql, parameters);
            }
        }

        public IEnumerable<ShiftCalendarDto> GetShiftCalendarsByShiftIds(List<int> shiftIds)
        {
            string executeSql = $@"SELECT StartTime,EndTime
                                   FROM ShiftCalendarsView
                                   WHERE MachineShiftDetailId IN @MachineShiftDetailId";

            using (var conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                return conn.Query<ShiftCalendarDto>(executeSql, new { MachineShiftDetailId = shiftIds });
            }
        }

        public async Task DeleteShiftOldData(MachineShiftEffectiveInterval input)
        {
            var detailSql = @"DELETE FROM  dbo.MachinesShiftDetails 
WHERE MachineId = @MachineId AND ShiftDay > @Today AND ShiftDay  BETWEEN @StartTime  AND @EndTime  
AND ShiftSolutionId = @ShiftSolutionId";

            var calendarSql = @"DELETE FROM  dbo.ShiftCalendars 
WHERE MachineId = @MachineId AND ShiftDay > @Today AND ShiftDay  BETWEEN @StartTime  AND @EndTime  
AND ShiftSolutionId = @ShiftSolutionId";


            using var conn = new SqlConnection(this.connectionString);
            await conn.OpenAsync();
            using var trans = conn.BeginTransaction();
            try
            {
                var param = new { input.MachineId, DateTime.Today, input.StartTime, input.EndTime, input.ShiftSolutionId };

                await conn.ExecuteAsync(detailSql, param, trans);
                await conn.ExecuteAsync(calendarSql, param, trans);

                trans.Commit();
            }
            catch (Exception)
            {
                trans.Rollback();
                throw new UserFriendlyException("清除旧数据出错!");
            }
        }

        public List<string> CheckIfCrossedWithLastedEffectiveShiftSolution(string startTime, string endTime)
        {
            var executeSql = $@"select ShiftDay from ShiftCalendars where ShiftDay <= DATEADD(DAY,-1,GETDATE()) and @StartTime < EndTime";
            var parameters = new { StartTime = startTime };

            using (var conn = new SqlConnection(this.connectionString))
            {
                conn.Open();

                return conn.Query<string>(executeSql, parameters).ToList();
            }
        }
    }
}
