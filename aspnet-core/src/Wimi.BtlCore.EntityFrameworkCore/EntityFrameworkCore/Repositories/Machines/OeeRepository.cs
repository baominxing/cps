namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.Machines
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    using Abp.Application.Services.Dto;
    using Abp.Configuration;
    using Dapper;
    using Wimi.BtlCore.CommonEnums;
    using Wimi.BtlCore.Configuration;
    using Wimi.BtlCore.OEE;

    public class OeeRepository : IOeeRepository
    {
        private readonly ISettingManager _settingManager;
        private readonly string connectionString;
        private readonly string capacityFilter;

        public OeeRepository(ISettingManager settingManager)
        {
            _settingManager = settingManager;
            connectionString = AppSettings.Database.ConnectionString;
            capacityFilter = Convert.ToBoolean(AppSettings.TraceabilityConfig.OfflineYield) ? "" : " AND c.IsLineOutput = 0 ";
        }

        public async Task<IEnumerable<OeeResponse>> ListMachineAvailability(OeeAnalysis input)
        {
            var sql = @"WITH planedTime AS (
                             SELECT T.Id,
                                    T.Name,
                                    T.ShiftDay,
                                    SUM(T.Duration)  AS Duration
                             FROM   (
                                        SELECT m.Id,
                                               m.Name,
                                               CONVERT(NVARCHAR, c.[Date], 23) AS ShiftDay,
                                               ISNULL(ssi.Duration, 0) * 60 AS Duration
                                        FROM   Machines AS m
                                               INNER JOIN MachinesShiftDetails AS msd  ON  msd.MachineId = m.Id
                                               INNER JOIN ShiftSolutionItems AS ssi  ON  msd.ShiftSolutionId = ssi.ShiftSolutionId
                                               RIGHT JOIN Calendars AS c ON  ( c.[Date] = msd.ShiftDay AND c.[Date] BETWEEN @StartTime AND @EndTime)
                                        WHERE m.Id IN @MachineIdList AND   msd.ShiftDay >= @StartTime AND msd.ShiftDay< @EndTime
                                        GROUP BY m.Id, ssi.Id, m.Name, c.[Date], ssi.Duration) AS T
                             GROUP BY T.Id, NAME, ShiftDay
                            ),
                             reasons AS (
                                SELECT rfr.MachineId,
                                         SUM(rfr.Duration) AS Duration,
                                         CONVERT(DATE, rfr.StartTime) AS 
                                         shiftDay
                                  FROM   ReasonFeedbackRecords AS rfr
                                         INNER JOIN StateInfos AS si ON  rfr.StateId = si.Id
                                  GROUP BY rfr.MachineId, CONVERT(DATE, rfr.StartTime)
                             ) 
     
                        SELECT pt.Id              AS machineId,
                               pt.Name            AS MachineName,
                               pt.ShiftDay,
                               pt.Duration        AS PlanedWorkingTime,
                               (pt.Duration - ISNULL(r.Duration, 0)) AS ActualWorkTime
                        FROM   planedTime         AS pt
                               LEFT JOIN reasons  AS r ON (pt.Id = r.MachineId AND pt.ShiftDay = r.ShiftDay)";

            using (var conn = new SqlConnection(this.connectionString))
            {
                var endTime = input.EndTime.AddDays(1);
                var result = await conn.QueryAsync<OeeResponse>(
                                 sql,
                                 new { input.MachineIdList, input.StartTime, endTime });
                return result;
            }
        }

        public async Task<IEnumerable<OeeResponse>> ListQualityIndicators(OeeAnalysis input)
        {
            var sql = $@"WITH cwith AS (
                             SELECT m.Id,
                                    m.Name,
                                    CONVERT(NVARCHAR,  c.ShiftDetail_ShiftDay, 23) as ShiftDetail_ShiftDay ,
                                    SUM(ISNULL(c.Yield, 0))  AS Yiled
                             FROM   Machines                 AS m
                                    INNER JOIN Capacities    AS c ON  m.Id = c.MachineId
                                    INNER JOIN MachinesShiftDetails AS msd ON c.MachinesShiftDetailId = msd.Id 
                             WHERE  m.Id IN @MachineIdList AND c.ShiftDetail_SolutionName IS NOT NULL
                                   AND c.ShiftDetail_ShiftDay  BETWEEN @StartTime  AND @EndTime {capacityFilter}
                             GROUP BY m.Id, m.Name,c.ShiftDetail_ShiftDay
                         ),
                         mdrWith AS (
                             SELECT CONVERT(DATE, mdr.ShiftDay) AS ShiftDay,
                                    mdr.MachineId,
                                    SUM(mdr.[Count])         AS COUNT
                             FROM   MachineDefectiveRecords  AS mdr
                             GROUP BY CONVERT(DATE, mdr.ShiftDay), mdr.MachineId
                         )
                    SELECT c.id                       AS MachineId,
                           c.Name                     AS MachineName,
                           c.ShiftDetail_ShiftDay     AS ShiftDay,
                           mp.Name                    AS ProductName,
                           mp.ProcessId               AS ProductId,
                           SUM(c.Yiled)               AS TotalCount,
                           SUM(ISNULL(m.[COUNT], 0))  AS UnqualifiedCount
                    FROM   cwith                      AS c
                           LEFT JOIN mdrWith          AS m ON  (c.Id = m.MachineId AND c.ShiftDetail_ShiftDay = m.ShiftDay)
                           LEFT JOIN (
                                    SELECT mp.MachineId,
                                           mp.ProcessId,
                                           p.Name
                                    FROM   MachineProcesses AS mp
                                           INNER JOIN Products AS p ON  mp.ProductId = p.Id
                                    WHERE  mp.IsDeleted = 0  AND p.IsDeleted = 0 AND mp.EndTime IS  NULL
                                )    AS mp ON  c.Id = mp.MachineId
                    GROUP BY  c.Id, c.Name, ShiftDetail_ShiftDay, mp.ProcessId, mp.Name ";

            using (var conn = new SqlConnection(this.connectionString))
            {
                var result = await conn.QueryAsync<OeeResponse>(sql, new { input.MachineIdList, input.StartTime, input.EndTime });
                return result;
            }
        }

        public async Task<IEnumerable<OeeResponse>> ListPerformanceIndicators(OeeAnalysis input)
        {
            var sql = $@"SELECT m.Name                             AS MachineName,
                           m.Id                               AS MachineId,
                           ISNULL((st.StandardCostTime / 60.0), 0) AS PerfectTime,
                           c.ProductId,
                           p.Name                             AS ProductName,
                           SUM(ISNULL(c.Yield, 0))            AS TotalYiled,
                           SUM(ISNULL(c.Duration, 0)) / 60.0  AS TotalDuration,
                           CONVERT(NVARCHAR, c.ShiftDetail_ShiftDay, 23) AS ShiftDay
                    FROM   Machines m 
                           INNER JOIN Capacities              AS c ON  m.Id = c.MachineId
                           INNER JOIN MachinesShiftDetails    AS msd ON  c.MachinesShiftDetailId = msd.Id
                           LEFT JOIN Products                 AS p ON  p.Id = c.ProductId
                           LEFT JOIN StandardTime AS st       ON (st.ProductId = c.ProductId AND st.ProcessId = c.ProcessId)
                    WHERE  m.Id IN @MachineIdList
                           AND c.ShiftDetail_SolutionName IS NOT NULL
                           AND c.ShiftDetail_ShiftDay BETWEEN @StartTime AND @EndTime  {capacityFilter}
                    GROUP BY c.ProductId,p.Name, m.Id,m.Name,c.ShiftDetail_ShiftDay,st.StandardCostTime ";

            using (var conn = new SqlConnection(this.connectionString))
            {
                var result = await conn.QueryAsync<OeeResponse>(sql, new { input.MachineIdList, input.StartTime, input.EndTime });
                return result;
            }
        }

        public async Task<IEnumerable<NameValueDto>> ListSummaryDate(OeeAnalysis input)
        {
            string selectSql;
            var innerSql = string.Empty;
            var groupBySql = string.Empty;
            switch (input.StatisticalWays)
            {
                case EnumStatisticalWays.ByWeek:
                    selectSql = " DISTINCT c.YYYYISOWeek+' 周' ";
                    break;
                case EnumStatisticalWays.ByMonth:
                    selectSql = " DISTINCT c.YYYYMM +' 月' ";
                    break;
                case EnumStatisticalWays.ByYear:
                    selectSql = " DISTINCT CAST(c.[Year] AS NVARCHAR) +' 年'";
                    break;
                case EnumStatisticalWays.ByShift:
                    selectSql = " CAST(c.[Month] AS NVARCHAR) + '-' + CAST(c.[Day] AS NVARCHAR) + ' ' + ssi.Name  ";
                    innerSql = @" INNER JOIN ShiftHistories AS msd   ON msd.ShiftDay = c.[Date]
                                  INNER JOIN ShiftSolutionItems AS ssi ON msd.ShiftSolutionItemId = ssi.Id";
                    groupBySql = " GROUP BY  c.[Month], c.[Day],c.[Date],ssi.Name,ssi.Id ";
                    break;
                default:
                    selectSql = " CONVERT(NVARCHAR, c.[Date], 23) ";
                    break;
            }

            var sql = $@"SELECT {selectSql} AS Value, CONVERT(NVARCHAR, c.[Date], 23) as Name
                        FROM   Calendars                        AS c
                        {innerSql}
                        WHERE  c.[Date] BETWEEN @StartTime AND @EndTime
                        {groupBySql} ";

            using (var conn = new SqlConnection(this.connectionString))
            {
                return await conn.QueryAsync<NameValueDto>(sql, new { input.StartTime, input.EndTime });
            }
        }

        public async Task<IEnumerable<int>> ListMachineIdInGroup(IEnumerable<int> groupIds)
        {
            var sql = @"SELECT mdg.MachineId FROM DeviceGroups AS dg 
                        INNER JOIN MachineDeviceGroups AS mdg ON mdg.DeviceGroupId = dg.Id 
                        WHERE dg.Id IN @GroupIds ";
            using (var conn = new SqlConnection(this.connectionString))
            {
                return await conn.QueryAsync<int>(sql, new { groupIds });
            }
        }

        public async Task<ShiftOeeResponse> ListShiftMachineOee(OeeAnalysis input)
        {
            var machineAvailabilitySql = @"WITH shift AS(
                             SELECT m.Id                  AS MachineId,
                                    m.Name                AS MachineName,
                                    c.[Date],
                                    CAST(c.[Month] AS NVARCHAR) + '-' + CAST(c.[Day] AS NVARCHAR) + 
                                    ' ' + ssi.Name        AS ShiftDay,
                                    ISNULL(ssi.Duration, 0) * 60.0 AS Duration,
                                    CONVERT(NVARCHAR, sh.ShiftDay, 23) + ' ' + CONVERT(NVARCHAR, ssi.StartTime, 114) AS 
                                    StartDate,
                                    CASE 
                                         WHEN ssi.EndTime > ssi.StartTime THEN CONVERT(NVARCHAR, sh.ShiftDay, 23) 
                                              + ' ' + CONVERT(NVARCHAR, ssi.EndTime, 114)
                                         ELSE CONVERT(NVARCHAR, DATEADD(DAY, 1, sh.ShiftDay), 23) + 
                                              ' ' + CONVERT(NVARCHAR, ssi.EndTime, 114)
                                    END                   AS EndDate
                             FROM   Machines              AS m
                                    INNER JOIN ShiftHistories AS sh ON  sh.MachineId = m.Id
                                    INNER JOIN ShiftSolutionItems AS ssi ON  sh.ShiftSolutionItemId = ssi.Id
                                    INNER JOIN Calendars  AS c ON  c.[Date] = sh.ShiftDay
                             WHERE  m.Id IN @MachineIdList and c.[Date] BETWEEN @StartTime AND @EndTime 
                             GROUP BY m.Id, m.Name,c.[Month],ssi.Name,c.[Day], ssi.Duration,c.[Date], sh.ShiftDay, ssi.StartTime,ssi.EndTime
                         )
                    SELECT s.MachineId,
                           s.MachineName,
                           SUM(ISNULL(rfr.Duration, 0)) AS ReasonDuration,
                           s.[Date],
                           S.ShiftDay,
                           s.Duration  AS TotalDuration
                    FROM   shift       AS s
                           LEFT JOIN ReasonFeedbackRecords AS rfr
                                ON  (
                                        s.MachineId = rfr.MachineId
                                        AND rfr.StartTime BETWEEN s.StartDate 
                                            AND s.EndDate
                                    )
                           LEFT JOIN StateInfos AS si ON  rfr.StateId = si.Id
                      GROUP BY s.MachineId, s.MachineName, s.[Date],S.ShiftDay, s.Duration ";

            var performanceSql = $@"WITH shift AS(
                                 SELECT m.Id                  AS MachineId,
                                        c.[Date],
                                        CAST(c.[Month] AS NVARCHAR) + '-' + CAST(c.[Day] AS NVARCHAR) +
                                        ' ' + ssi.Name        AS ShiftDay,
                                        ISNULL(ssi.Duration, 0) * 60.0 AS Duration,
                                        CONVERT(NVARCHAR, msd.ShiftDay, 23) + ' ' + CONVERT(NVARCHAR, ssi.StartTime, 114) AS 
                                        StartDate,
                                        CASE 
                                             WHEN ssi.EndTime > ssi.StartTime THEN CONVERT(NVARCHAR, msd.ShiftDay, 23) 
                                                  + ' ' + CONVERT(NVARCHAR, ssi.EndTime, 114)
                                             ELSE CONVERT(NVARCHAR, DATEADD(DAY, 1, msd.ShiftDay), 23) +
                                                  ' ' + CONVERT(NVARCHAR, ssi.EndTime, 114)
                                        END                   AS EndDate
                                 FROM   Machines              AS m
                                        INNER JOIN ShiftHistories AS msd ON  msd.MachineId = m.Id
                                        INNER JOIN ShiftSolutionItems AS ssi ON  msd.ShiftSolutionItemId = ssi.Id
                                        INNER JOIN Calendars  AS c ON  c.[Date] = msd.ShiftDay
                                 WHERE  m.Id IN @MachineIdList AND c.[Date] BETWEEN @StartTime AND @EndTime
                                 GROUP BY m.Id,c.[Month],ssi.Name,c.[Day], ssi.Duration, c.[Date], msd.ShiftDay,ssi.StartTime,ssi.EndTime)
                        
                        SELECT T.MachineId,
                               T.MachineName,
                               T.ShiftDay,
                               T.[Date],
                               SUM(T.TotalYiled)     AS TotalYiled,
                               SUM(T.TotalDuration)  AS TotalDuration,
                               SUM(T.PerfectTime)    AS PerfectTime
                        FROM   (
                                   SELECT m.Id                    AS MachineId,
                                          m.Name                  AS MachineName,
                                          s.ShiftDay,
                                          s.[Date],
                                          SUM(ISNULL(c.Yield, 0)) AS TotalYiled,
                                          SUM(ISNULL(c.Duration, 0)) / 60.0 AS TotalDuration,
                                          ISNULL(st.StandardCostTime, 0) / 60.0 AS PerfectTime
                                   FROM   Machines                AS m
                                          INNER JOIN Capacities   AS c ON  m.Id = c.MachineId
                                          INNER JOIN shift        AS s
                                           ON  (
                                                       s.MachineId = c.MachineId
                                                       AND c.StartTime BETWEEN s.StartDate AND s.EndDate
                                                       AND c.ShiftDetail_ShiftDay IS NOT NULL
                                                   )
                                          LEFT JOIN StandardTime  AS st ON  (st.ProductId = c.ProductId AND st.ProcessId = c.ProcessId)
                                   WHERE 1=1 {capacityFilter}
                                   GROUP BY m.Id, m.Name,s.ShiftDay,s.[Date],st.StandardCostTime )                     AS T
                        GROUP BY T.MachineId,T.MachineName,T.ShiftDay, T.[Date] ";

            var qualityIndicatorsSql = $@"WITH shift AS(
	                                SELECT m.Id                  AS MachineId,
                                    ssi.Id                AS ShiftSolutionItemId,
                                    msd.MachineShiftDetailId ,
                                    c.[Date],
                                    CAST(c.[Month] AS NVARCHAR) + '-' + CAST(c.[Day] AS NVARCHAR) +
                                    ' ' + ssi.Name        AS ShiftDay,
                                    ISNULL(ssi.Duration, 0) * 60 AS Duration,
                                    CONVERT(NVARCHAR, msd.ShiftDay, 23) + ' ' + CONVERT(NVARCHAR, ssi.StartTime, 114) AS 
                                    StartDate,
                                    CASE 
                                         WHEN ssi.EndTime > ssi.StartTime THEN CONVERT(NVARCHAR, msd.ShiftDay, 23) 
                                              + ' ' + CONVERT(NVARCHAR, ssi.EndTime, 114)
                                         ELSE CONVERT(NVARCHAR, DATEADD(DAY, 1, msd.ShiftDay), 23) +
                                              ' ' + CONVERT(NVARCHAR, ssi.EndTime, 114)
                                    END                   AS EndDate
                             FROM   Machines              AS m
                                    INNER JOIN ShiftHistories AS msd ON  msd.MachineId = m.Id
                                    INNER JOIN ShiftSolutionItems AS ssi ON  msd.ShiftSolutionItemId = ssi.Id
                                    INNER JOIN Calendars  AS c ON  c.[Date] = msd.ShiftDay
                            WHERE m.Id IN @MachineIdList and c.[Date] BETWEEN @StartTime AND @EndTime 
                             GROUP BY m.Id,ssi.Id,msd.MachineShiftDetailId,c.[Month],ssi.Name, c.[Day],ssi.Duration,c.[Date], msd.ShiftDay,ssi.StartTime,ssi.EndTime
                         ),
                         perfectTime AS
                         (
                             SELECT mp.MachineId,
                                    p.id,
                                    p.Name
                             FROM   MachineProcesses     AS mp
                                    INNER JOIN Products  AS p ON  mp.ProductId = p.Id
                             WHERE  mp.EndTime IS NULL AND mp.IsDeleted = 0
                         )
                
                    SELECT T.MachineId,
                           T.[Date],
                           T.ShiftDay,
                           T.Duration,
                           T.StartDate,
                           T.EndDate,
                           SUM(ISNULL(T.Yield, 0))  AS TotalCount,
                           ISNULL(B.[Count],0)      AS UnqualifiedCount,
                           p.Name                   AS ProductName,
                           P.Id                     AS ProductId
                    FROM   (
                               SELECT DISTINCT s.MachineId,
                                      s.[Date],
                                      c.Id,
                                      s.ShiftDay,
                                      s.Duration,
                                      s.StartDate,
                                      s.EndDate,
                                      s.ShiftSolutionItemId,
                                      c.Yield
                               FROM   shift                  AS s
                                      INNER JOIN Capacities  AS c
                                           ON  (
                                                   c.MachineId = s.MachineId
                                                   AND c.MachinesShiftDetailId = s.MachineShiftDetailId
                                                   AND c.ShiftDetail_ShiftDay IS NOT NULL 
                                               )
                                     WHERE 1=1 {capacityFilter}
                           )                        AS T
                           LEFT JOIN (
                                    SELECT mdr.MachineId,
                                           ISNULL(SUM(mdr.[Count]),0)  AS [Count],
                                           mdr.ShiftSolutionItemId,
                                           mdr.ShiftDay
                                    FROM   MachineDefectiveRecords AS mdr
                                           INNER JOIN shift  AS s
                                                ON  (   
                                                        mdr.MachineId = s.MachineId
                                                        AND mdr.ShiftSolutionItemId = s.ShiftSolutionItemId
                                                        AND mdr.ShiftDay = s.[Date]
                                                    )
                                    GROUP BY mdr.MachineId,mdr.ShiftSolutionItemId,mdr.ShiftDay
                                )                   AS B ON  (B.MachineId = T.MachineId AND B.ShiftSolutionItemId = T.ShiftSolutionItemId AND B.ShiftDay = T.[Date])
                           LEFT JOIN perfectTime    AS p ON  t.MachineId = p.MachineId
                    GROUP BY T.MachineId,T.[Date],T.ShiftDay,T.Duration,T.StartDate, T.EndDate,p.Name, p.id,B.[Count]   ";

            using (var conn = new SqlConnection(this.connectionString))
            {
                var param = new { input.MachineIdList, input.StartTime, input.EndTime };
                return new ShiftOeeResponse
                {
                    Availability = await conn.QueryAsync<OeeResponse>(machineAvailabilitySql, param),
                    Performance = await conn.QueryAsync<OeeResponse>(performanceSql, param),
                    QualityIndicators = await conn.QueryAsync<OeeResponse>(qualityIndicatorsSql, param)
                };
            }
        }

        public async Task<IEnumerable<ShiftDateRange>> ListShiftDateTimeRange(OeeAnalysis input)
        {
            var sql = @"SELECT c.[Date],
                       CAST(c.[Month] AS NVARCHAR) + '-' + CAST(c.[Day] AS NVARCHAR) +
                       ' ' + ssi.Name                 AS ShiftDay,
                       CONVERT(NVARCHAR, msd.ShiftDay, 23) + ' ' + CONVERT(NVARCHAR, ssi.StartTime, 108) AS 
                       StartTime,
                       CASE 
                            WHEN ssi.EndTime > ssi.StartTime THEN CONVERT(NVARCHAR, msd.ShiftDay, 23) 
                                 + ' ' + CONVERT(NVARCHAR, ssi.EndTime, 108)
                            ELSE CONVERT(NVARCHAR, DATEADD(DAY, 1, msd.ShiftDay), 23) +
                                 ' ' + CONVERT(NVARCHAR, ssi.EndTime, 108)
                       END                            AS EndTime
                FROM   MachinesShiftDetails           AS msd
                       INNER JOIN ShiftSolutionItems  AS ssi ON  msd.ShiftSolutionId = ssi.ShiftSolutionId
                       INNER JOIN Calendars           AS c ON  c.[Date] = msd.ShiftDay
                WHERE  c.[Date] BETWEEN @StartTime AND @EndTime
                GROUP BY c.[Month], ssi.Name,c.[Day], ssi.Duration, c.[Date], msd.ShiftDay, ssi.StartTime, ssi.EndTime";

            using (var conn = new SqlConnection(this.connectionString))
            {
                return await conn.QueryAsync<ShiftDateRange>(sql, new { input.StartTime, input.EndTime });
            }
        }

        public async Task<IEnumerable<OeeResponse>> ListQualityIndicatorsItemByProduct(OeeAnalysis input)
        {
            var sql = $@"WITH cwith AS (
                             SELECT m.Name,
                                    m.Id,
                                    c.ProductId,
                                    p.Name                   AS ProductName,
                                    SUM(ISNULL(c.Yield, 0))  AS Yield,
                                    CONVERT(NVARCHAR, c.ShiftDetail_ShiftDay, 23) AS 
                                    ShiftDetail_ShiftDay
                             FROM   Machines m
                                    INNER JOIN Capacities    AS c ON  m.Id = c.MachineId
                                    INNER JOIN MachinesShiftDetails AS msd ON  c.MachinesShiftDetailId = msd.Id
                                    LEFT JOIN Products       AS p ON  p.Id = c.ProductId
                             WHERE  m.Id IN @MachineIdList
                                    AND c.ShiftDetail_SolutionName IS NOT NULL
                                    AND c.ShiftDetail_ShiftDay BETWEEN @StartTime AND @EndTime {capacityFilter}
                             GROUP BY c.ProductId,p.Name, m.Id, m.Name, c.ShiftDetail_ShiftDay
                         ),
                         mdrWith AS (
                             SELECT CONVERT(DATE, mdr.ShiftDay) AS ShiftDay,
                                    mdr.MachineId,
                                    mdr.ProductId,
                                    SUM(mdr.[Count])         AS COUNT
                             FROM   MachineDefectiveRecords  AS mdr
                             GROUP BY CONVERT(DATE, mdr.ShiftDay), mdr.MachineId, mdr.ProductId
                         )

                    SELECT c.id                    AS MachineId,
                           c.Name                  AS MachineName,
                           c.ShiftDetail_ShiftDay  AS ShiftDay,
                           c.ProductName           AS ProductName,
                           c.ProductId             AS ProductId,
                           c.Yield                 AS TotalCount,
                           ISNULL(m.[COUNT], 0)    AS UnqualifiedCount
                    FROM   cwith                   AS c
                           LEFT JOIN mdrWith       AS m
                                ON  (
                                        c.Id = m.MachineId
                                        AND c.ShiftDetail_ShiftDay = m.ShiftDay
                                        AND c.ProductId = m.ProductId
                                    )  ";

            using (var conn = new SqlConnection(this.connectionString))
            {
                var result = await conn.QueryAsync<OeeResponse>(sql, new { input.MachineIdList, input.StartTime, input.EndTime });
                return result;
            }
        }

        public async Task<IEnumerable<OeeResponse>> ListPerformanceIndicatorsItemByProduct(OeeAnalysis input)
        {
            var sql = $@"SELECT m.Name                             AS MachineName,
                           m.Id                               AS MachineId,
                           ISNULL((st.StandardCostTime / 60.0), 0) AS PerfectTime,
                           c.ProductId,
                           p.Name                             AS ProductName,
                           SUM(ISNULL(c.Yield, 0))            AS TotalYiled,
                           SUM(ISNULL(c.Duration, 0)) / 60.0  AS TotalDuration,
                           CONVERT(NVARCHAR, c.ShiftDetail_ShiftDay, 23) AS ShiftDay
                    FROM   Machines m 
                           INNER JOIN Capacities              AS c ON  m.Id = c.MachineId
                           INNER JOIN MachinesShiftDetails    AS msd ON  c.MachinesShiftDetailId = msd.Id
                           LEFT JOIN Products                 AS p ON  p.Id = c.ProductId
                           LEFT JOIN StandardTime AS st ON (st.ProcessId = c.ProcessId AND st.ProductId = c.ProductId)
                    WHERE  m.Id IN @MachineIdList
                           AND c.ShiftDetail_SolutionName IS NOT NULL
                           AND c.ShiftDetail_ShiftDay BETWEEN @StartTime AND  @EndTime {capacityFilter}
                    GROUP BY c.ProductId,p.Name, m.Id,m.Name,c.ShiftDetail_ShiftDay,st.StandardCostTime ";

            using (var conn = new SqlConnection(this.connectionString))
            {
                var result = await conn.QueryAsync<OeeResponse>(sql, new { input.MachineIdList, input.StartTime, input.EndTime });
                return result;
            }
        }

        public async Task<ShiftOeeResponse> ListShiftMachineOeeByProduct(OeeAnalysis input)
        {
            var machineAvailabilitySql = @"WITH shift AS(
                             SELECT m.Id                  AS MachineId,
                                    m.Name                AS MachineName,
                                    c.[Date],
                                    CAST(c.[Month] AS NVARCHAR) + '-' + CAST(c.[Day] AS NVARCHAR) + 
                                    ' ' + ssi.Name        AS ShiftDay,
                                    ISNULL(ssi.Duration, 0) * 60.0 AS Duration,
                                    CONVERT(NVARCHAR, sh.ShiftDay, 23) + ' ' + CONVERT(NVARCHAR, ssi.StartTime, 114) AS 
                                    StartDate,
                                    CASE 
                                         WHEN ssi.EndTime > ssi.StartTime THEN CONVERT(NVARCHAR, sh.ShiftDay, 23) 
                                              + ' ' + CONVERT(NVARCHAR, ssi.EndTime, 114)
                                         ELSE CONVERT(NVARCHAR, DATEADD(DAY, 1, sh.ShiftDay), 23) + 
                                              ' ' + CONVERT(NVARCHAR, ssi.EndTime, 114)
                                    END                   AS EndDate
                             FROM   Machines              AS m
                                    INNER JOIN ShiftHistories AS sh ON  sh.MachineId = m.Id
                                    INNER JOIN ShiftSolutionItems AS ssi ON  sh.ShiftSolutionItemId = ssi.Id
                                    INNER JOIN Calendars  AS c ON  c.[Date] = sh.ShiftDay
                             WHERE  m.Id IN @MachineIdList and c.[Date] BETWEEN @StartTime AND @EndTime 
                             GROUP BY m.Id, m.Name,c.[Month],ssi.Name,c.[Day], ssi.Duration,c.[Date], sh.ShiftDay, ssi.StartTime,ssi.EndTime
                         )
                    SELECT s.MachineId,
                           s.MachineName,
                           SUM(ISNULL(rfr.Duration, 0)) AS ReasonDuration,
                           s.[Date],
                           S.ShiftDay,
                           s.Duration  AS TotalDuration
                    FROM   shift       AS s
                           LEFT JOIN ReasonFeedbackRecords AS rfr
                                ON  (
                                        s.MachineId = rfr.MachineId
                                        AND rfr.StartTime BETWEEN s.StartDate 
                                            AND s.EndDate
                                    )
                           LEFT JOIN StateInfos AS si ON  rfr.StateId = si.Id
                      GROUP BY s.MachineId, s.MachineName, s.[Date],S.ShiftDay, s.Duration ";

            var qualitySql = $@"WITH shift AS( SELECT m.Id                  AS MachineId,
                                    ssi.Id                AS ShiftSolutionItemId,
                                    msd.MachineShiftDetailId ,
                                    c.[Date],
                                    CAST(c.[Month] AS NVARCHAR) + '-' + CAST(c.[Day] AS NVARCHAR) +
                                    ' ' + ssi.Name        AS ShiftDay,
                                    ISNULL(ssi.Duration, 0) * 60 AS Duration,
                                    CONVERT(NVARCHAR, msd.ShiftDay, 23) + ' ' + CONVERT(NVARCHAR, ssi.StartTime, 114) AS 
                                    StartDate,
                                    CASE 
                                         WHEN ssi.EndTime > ssi.StartTime THEN CONVERT(NVARCHAR, msd.ShiftDay, 23) 
                                              + ' ' + CONVERT(NVARCHAR, ssi.EndTime, 114)
                                         ELSE CONVERT(NVARCHAR, DATEADD(DAY, 1, msd.ShiftDay), 23) +
                                              ' ' + CONVERT(NVARCHAR, ssi.EndTime, 114)
                                    END                   AS EndDate
                             FROM   Machines              AS m
                                    INNER JOIN ShiftHistories AS msd ON  msd.MachineId = m.Id
                                    INNER JOIN ShiftSolutionItems AS ssi ON  msd.ShiftSolutionItemId = ssi.Id
                                    INNER JOIN Calendars  AS c ON  c.[Date] = msd.ShiftDay
                            WHERE m.Id IN @MachineIdList and c.[Date]   BETWEEN @StartTime AND @EndTime 
                             GROUP BY m.Id,ssi.Id,msd.MachineShiftDetailId,c.[Month],ssi.Name, c.[Day],ssi.Duration,c.[Date], msd.ShiftDay,ssi.StartTime,ssi.EndTime
                         )
                
                    SELECT T.MachineId,
                           T.[Date],
                           T.ShiftDay,
                           T.Duration,
                           T.StartDate,
                           T.EndDate,
                           SUM(ISNULL(T.Yield, 0))  AS TotalCount,
                           ISNULL(B.[Count],0)      AS UnqualifiedCount,
                           p.Name                   AS ProductName,
                           P.Id                     AS ProductId
                    FROM   (
                               SELECT DISTINCT s.MachineId,
                                      s.[Date],
                                      c.Id,
                                      s.ShiftDay,
                                      s.Duration,
                                      s.StartDate,
                                      s.EndDate,
                                      s.ShiftSolutionItemId,
                                      c.Yield,
                                      c.ProductId
                               FROM   shift                  AS s
                                      INNER JOIN Capacities  AS c
                                           ON  (
                                                   c.MachineId = s.MachineId
                                                   AND c.MachinesShiftDetailId = s.MachineShiftDetailId
                                                   AND c.ShiftDetail_ShiftDay IS NOT NULL 
                                           )
                                      WHERE 1=1 {capacityFilter}
                           )                        AS T
                           LEFT JOIN (
                                    SELECT mdr.MachineId,
                                           ISNULL(SUM(mdr.[Count]),0)  AS [Count],
                                           mdr.ShiftSolutionItemId,
                                           mdr.ShiftDay,
                                           mdr.ProductId
                                    FROM   MachineDefectiveRecords AS mdr
                                           INNER JOIN shift  AS s
                                                ON  (   
                                                        mdr.MachineId = s.MachineId
                                                        AND mdr.ShiftSolutionItemId = s.ShiftSolutionItemId
                                                        AND mdr.ShiftDay = s.[Date]
                                                    )
                                    GROUP BY mdr.MachineId,mdr.ShiftSolutionItemId,mdr.ShiftDay, mdr.ProductId
                                ) AS B ON  (B.MachineId = T.MachineId AND B.ShiftSolutionItemId = T.ShiftSolutionItemId AND B.ShiftDay = T.[Date] AND B.ProductId = t.ProductId)
                           LEFT JOIN Products AS p  ON  t.ProductId = p.id
                    GROUP BY T.MachineId,T.[Date],T.ShiftDay,T.Duration,T.StartDate, T.EndDate,p.Name, p.id,B.[Count]";

            var performanceSql = $@"WITH shift AS(
                             SELECT m.Id                  AS MachineId,
                                    c.[Date],
                                    CAST(c.[Month] AS NVARCHAR) + '-' + CAST(c.[Day] AS NVARCHAR) +
                                    ' ' + ssi.Name        AS ShiftDay,
                                    ISNULL(ssi.Duration, 0) * 60.0 AS Duration,
                                    CONVERT(NVARCHAR, msd.ShiftDay, 23) + ' ' + CONVERT(NVARCHAR, ssi.StartTime, 114) AS 
                                    StartDate,
                                    CASE 
                                         WHEN ssi.EndTime > ssi.StartTime THEN CONVERT(NVARCHAR, msd.ShiftDay, 23) 
                                              + ' ' + CONVERT(NVARCHAR, ssi.EndTime, 114)
                                         ELSE CONVERT(NVARCHAR, DATEADD(DAY, 1, msd.ShiftDay), 23) +
                                              ' ' + CONVERT(NVARCHAR, ssi.EndTime, 114)
                                    END                   AS EndDate
                             FROM   Machines              AS m
                                    INNER JOIN ShiftHistories AS msd  ON  msd.MachineId = m.Id
                                    INNER JOIN ShiftSolutionItems AS ssi ON  msd.ShiftSolutionItemId = ssi.Id
                                    INNER JOIN Calendars  AS c  ON  c.[Date] = msd.ShiftDay
                             WHERE  m.Id IN @MachineIdList AND c.[Date] BETWEEN @StartTime AND  @EndTime
                             GROUP BY m.Id,c.[Month], ssi.Name,c.[Day],ssi.Duration,c.[Date],msd.ShiftDay,ssi.StartTime,ssi.EndTime
                         )

                    SELECT m.Id                               AS MachineId,
                           m.Name                             AS MachineName,
                           s.ShiftDay,
                           s.[Date],
                           SUM(ISNULL(c.Yield, 0))            AS TotalYiled,
                           SUM(ISNULL(c.Duration, 0)) / 60.0  AS TotalDuration,
                           ISNULL(st.StandardCostTime, 0) / 60.0 AS PerfectTime,
                           c.ProductId,
                           p.Name                             AS ProductName
                    FROM   Machines                           AS m
                           INNER JOIN Capacities              AS c ON  m.Id = c.MachineId
                           INNER JOIN shift                   AS s
                                ON  (
                                        s.MachineId = c.MachineId
                                        AND c.StartTime BETWEEN s.StartDate AND s.EndDate
                                        AND c.ShiftDetail_ShiftDay IS NOT NULL
                                    )
                           LEFT JOIN Products                 AS p ON  c.ProductId = p.id
                           LEFT JOIN StandardTime             AS st ON  (st.ProductId = c.ProductId AND st.ProcessId = c.ProcessId)
                    WHERE 1=1 {capacityFilter}
                    GROUP BY m.Id,m.Name,s.ShiftDay, s.[Date],st.StandardCostTime,p.Name,c.ProductId";
            using (var conn = new SqlConnection(this.connectionString))
            {
                var param = new { input.MachineIdList, input.StartTime, input.EndTime };
                return new ShiftOeeResponse
                           {
                               Availability = await conn.QueryAsync<OeeResponse>(machineAvailabilitySql, param),
                               QualityIndicators = await conn.QueryAsync<OeeResponse>(qualitySql, param),
                               Performance = await conn.QueryAsync<OeeResponse>(performanceSql, param)
                           };
            }
        }

        public async Task<IEnumerable<UnplannedPause>> ListUnplannedPause(int machineId, DateTime? startTime, DateTime? endTime)
        {
            var sql = @"SELECT  rfr.StateCode,
                               si.DisplayName as Name,
                               si.Hexcode,
                               rfr.Duration
                        FROM   ReasonFeedbackRecords  AS rfr
                               INNER JOIN StateInfos  AS si ON  si.Id = rfr.StateId
                        WHERE  rfr.machineId =@MachineId And rfr.StartTime >= @StartTime AND  rfr.StartTime < @EndTime ";

            using (var conn = new SqlConnection(this.connectionString))
            {
                return await conn.QueryAsync<UnplannedPause>(
                           sql,
                           new
                           {
                               machineId,
                               StartTime = startTime ?? DateTime.Today,
                               endTime = endTime ?? DateTime.Today
                           });
            }
        }

        public async Task<IEnumerable<string>> ListRevisedDate(EnumStatisticalWays type, DateTime startTime, DateTime endTime)
        {
            string selectSql;
            switch (type)
            {
                case EnumStatisticalWays.ByWeek:
                    selectSql = "c.YYYYISOWeek";
                    break;
                case EnumStatisticalWays.ByMonth:
                    selectSql = "c.YYYYMM";
                    break;
                case EnumStatisticalWays.ByYear:
                    selectSql = "c.[Year]";
                    break;

                default:
                    selectSql = "c.[Date]";
                    break;
            }
            var sql = $@" SELECT  CONVERT(NVARCHAR, c.[Date], 23) FROM   Calendars AS c
                         WHERE  {selectSql} IN (SELECT {selectSql} FROM   Calendars AS c
                                 WHERE  c.[Date] BETWEEN  @StartTime AND @EndTime)";

            using (var conn = new SqlConnection(this.connectionString))
            {
                return await conn.QueryAsync<string>(sql, new { startTime, endTime });
            }
        }
    }
}