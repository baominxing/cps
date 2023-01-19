using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Dashboard;
using Wimi.BtlCore.Dashboard.Dtos;

namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.Dashboard
{
    public class DashboardRepository : IDashboardRepository
    {
        public async Task<IEnumerable<MachineStatisticDataDto>> GetDashboardStatisticInGivenDays(GetStatesStatisticInGivenDaysInputDto input)
        {
            var rawSql = @"WITH cteCalendar AS (
                            SELECT c.[Date]
                            FROM   calendars c
                            WHERE  c.DateKey >= @DateFromInt
                                AND c.DateKey <= @DateEndInt
                        ),
                        cteStatesSummary AS(
                            SELECT CASE 
                                        WHEN ISNULL(SUM(ss.TotalDuration), 0) = 0 THEN 0
                                        ELSE CONVERT(
                                                DECIMAL(18, 2),
                                                SUM(ss.DebugDuration) / SUM(ss.TotalDuration) * 
                                                100
                                            )
                                END                     AS DebugRate,
                                CASE 
                                        WHEN ISNULL(SUM(ss.TotalDuration), 0) = 0 THEN 0
                                        ELSE CONVERT(
                                                DECIMAL(18, 2),
                                                SUM(ss.OfflineDuration) / SUM(ss.TotalDuration) * 
                                                100
                                            )
                                END                     AS OfflineRate,
                                CASE 
                                        WHEN ISNULL(SUM(ss.TotalDuration), 0) = 0 THEN 0
                                        ELSE CONVERT(
                                                DECIMAL(18, 2),
                                                SUM(ss.FreeDuration) / SUM(ss.TotalDuration) * 100
                                            )
                                END                     AS FreeRate,
                                CASE 
                                        WHEN ISNULL(SUM(ss.TotalDuration), 0) = 0 THEN 0
                                        ELSE CONVERT(
                                                DECIMAL(18, 2),
                                                SUM(ss.RunDuration) / SUM(ss.TotalDuration) * 100
                                            )
                                END                     AS RunRate,
                                CASE 
                                        WHEN ISNULL(SUM(ss.TotalDuration), 0) = 0 THEN 0
                                        ELSE CONVERT(
                                                DECIMAL(18, 2),
                                                SUM(ss.StopDuration) / SUM(ss.TotalDuration) * 100
                                            )
                                END                     AS StopRate,
                                CONVERT(NVARCHAR(10), ss.[ShiftDay], 23) SummaryDate
                            FROM   DailyStatesSummaries    AS ss
                                INNER JOIN cteCalendar  AS c
                                        ON  CONVERT(NVARCHAR(10), ss.[ShiftDay], 23) = c.[Date]
                            WHERE  ss.MachineId = @MachineId
                            GROUP BY
                                ss.[ShiftDay]
                        ),
                        cteCapacity AS(
                            SELECT CONVERT(VARCHAR, CONVERT(DATE, c.EndTime)) [SummaryDate],
                                SUM(c.Yield)               yield
                            FROM   Capacities              AS c
                                INNER JOIN cteCalendar  AS cc
                                        ON  c.EndTime >= cc.[Date]
                                        AND c.EndTime <= DATEADD(dd, 1, cc.DATE)
                            WHERE  c.MachineId = @MachineId
                            GROUP BY
                                CONVERT(VARCHAR, CONVERT(DATE, c.EndTime))
                        )

                SELECT CONVERT(VARCHAR(5), cc.[Date], 1)  AS SummaryDate,
                        ISNULL(css.DebugRate, 0)           AS DebugRate,
                        ISNULL(css.OfflineRate, 0)         AS OfflineRate,
                        ISNULL(css.FreeRate, 0)            AS FreeRate,
                        ISNULL(css.RunRate, 0)             AS RunRate,
                        ISNULL(css.StopRate, 0)            AS StopRate,
                        ISNULL(ccy.yield, 0)               AS yield
                FROM   cteCalendar cc
                        LEFT JOIN cteStatesSummary         AS css  ON  cc.[Date] = css.[SummaryDate]
                        LEFT JOIN cteCapacity ccy ON  cc.[Date] = ccy.[SummaryDate]
                ORDER BY  SummaryDate";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();
                var queryDtos =
                    await
                    conn.QueryAsync<MachineStatisticDataDto>(
                        rawSql,
                        new { DateFromInt = input.DateFrom, DateEndInt = input.DateEnd, input.MachineId });

                return queryDtos;
            }
        }

        public async Task<IEnumerable<MachineStatisticDataDto>> GetDashboardStatisticInGivenDaysByGroupId(GetStatesStatisticInGivenDaysInputDto input)
        {
            var rawSql = @"WITH cteCalendar AS (
                                 SELECT c.[Date]
                                 FROM   Calendars c
                                 WHERE  c.DateKey >= @DateFromInt
                                        AND c.DateKey <= @DateEndInt
                             ),
                             cteStatesSummary AS(
                                 SELECT 
                                        CASE 
                                             WHEN ISNULL(SUM(ss.TotalDuration), 0) = 0 THEN 0
                                             ELSE CONVERT(
                                                      DECIMAL(18, 2),
                                                      SUM(ss.DebugDuration) / SUM(ss.TotalDuration) * 100
                                                  )
                                        END                     AS DebugRate,
                                        CASE 
                                             WHEN ISNULL(SUM(ss.TotalDuration), 0) = 0 THEN 0
                                             ELSE CONVERT(
                                                      DECIMAL(18, 2),
                                                      SUM(ss.FreeDuration) / SUM(ss.TotalDuration) * 100
                                                  )
                                        END                     AS FreeRate,
                                        CASE 
                                             WHEN ISNULL(SUM(ss.TotalDuration), 0) = 0 THEN 0
                                             ELSE CONVERT(
                                                      DECIMAL(18, 2),
                                                      SUM(ss.RunDuration) / SUM(ss.TotalDuration) * 
                                                      100
                                                  )
                                        END                     AS RunRate,
                                        CASE 
                                             WHEN ISNULL(SUM(ss.TotalDuration), 0) = 0 THEN 0
                                             ELSE CONVERT(
                                                      DECIMAL(18, 2),
                                                      SUM(ss.OfflineDuration) / SUM(ss.TotalDuration) * 
                                                      100
                                                  )
                                        END                     AS OfflineRate,
                                        CASE 
                                             WHEN ISNULL(SUM(ss.TotalDuration), 0) = 0 THEN 0
                                             ELSE CONVERT(
                                                      DECIMAL(18, 2),
                                                      SUM(ss.StopDuration) / SUM(ss.TotalDuration) * 
                                                      100
                                                  )
                                        END                     AS StopRate,
                                        ss.ShiftDay AS [SummaryDate]
                                 FROM   DailyStatesSummaries    AS ss
                                        INNER JOIN cteCalendar  AS c ON  ss.ShiftDay = c.[Date]
                                        INNER JOIN MachineDeviceGroups AS mdg ON  mdg.MachineId = ss.MachineId
                                 WHERE  mdg.DeviceGroupId = @GroupId
                                 GROUP BY
                                        ss.ShiftDay
                             )

                        SELECT CONVERT(VARCHAR(5), cc.[Date], 1)  AS SummaryDate,
                               ISNULL(css.DebugRate, 0)           AS DebugRate,                               
                               ISNULL(css.FreeRate, 0)            AS FreeRate,
                               ISNULL(css.RunRate, 0)             AS RunRate,
                               ISNULL(css.OfflineRate, 0)         AS OfflineRate,
                               ISNULL(css.StopRate, 0)            AS StopRate
                        FROM   cteCalendar cc
                               LEFT JOIN cteStatesSummary         AS css
                                    ON  cc.[Date] = css.[SummaryDate]
                        ORDER BY cc.[Date]";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();
                conn.Execute(
                    "sp_CurrentStateDurationStatistics",
                    new { MachineIdList = (int?)null },
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120);

                var queryDtos =
                    await
                    conn.QueryAsync<MachineStatisticDataDto>(
                        rawSql,
                        new { DateFromInt = input.DateFrom, DateEndInt = input.DateEnd, input.GroupId });

                return queryDtos;
            }
        }

        public async Task<MachineStatisticDataDto> GetMachineStatisticData(int machineId)
        {
            var resultDto = new MachineStatisticDataDto();
 
            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                var sql = @"SELECT CASE 
                            WHEN SUM(ss.TotalDuration) = 0 THEN 0
                            ELSE CAST(
                                        CONVERT(DECIMAL(18, 2), SUM(ss.DebugDuration)) / SUM(ss.TotalDuration) 
                                        *
                                        100 AS DECIMAL(18, 2)
                                    )
                        END  AS DebugRate,
                        CASE 
                            WHEN SUM(ss.TotalDuration) = 0 THEN 0
                            ELSE CAST(
                                        CONVERT(DECIMAL(18, 2), SUM(ss.FreeDuration)) / SUM(ss.TotalDuration) 
                                        *
                                        100 AS DECIMAL(18, 2)
                                    )
                        END  AS FreeRate,
                        CASE 
                            WHEN SUM(ss.TotalDuration) = 0 THEN 0
                            ELSE CAST(
                                        CONVERT(DECIMAL(18, 2), SUM(ss.RunDuration)) / SUM(ss.TotalDuration) 
                                        *
                                        100 AS DECIMAL(18, 2)
                                    )
                        END  AS RunRate,
                        CASE 
                            WHEN SUM(ss.TotalDuration) = 0 THEN 0
                            ELSE CAST(
                                        CONVERT(DECIMAL(18, 2), SUM(ss.OfflineDuration)) / SUM(ss.TotalDuration) 
                                        *
                                        100 AS DECIMAL(18, 2)
                                    )
                        END  AS OfflineRate,
                        CASE 
                            WHEN SUM(ss.TotalDuration) = 0 THEN 0
                            ELSE CAST(
                                        CONVERT(DECIMAL(18, 2), SUM(ss.StopDuration)) / SUM(ss.TotalDuration) 
                                        *
                                        100 AS DECIMAL(18, 2)
                                    )
                        END  AS StopRate
                FROM   func_GetStateSummeryByDate(@DateFrom, @DateEnd, @MachineIdList) ss";

                conn.Open();
                var queryDto =
                    await
                    conn.QueryFirstOrDefaultAsync<MachineStatisticDataDto>(
                        sql,
                        new
                        {
                            DateFrom = DateTime.Today,
                            DateEnd = DateTime.Now,
                            MachineIdList = machineId.ToString()
                        });

                resultDto.StopRate = queryDto == null ? 0 : queryDto.StopRate;
                resultDto.FreeRate = queryDto == null ? 0 : queryDto.FreeRate;
                resultDto.RunRate = queryDto == null ? 0 : queryDto.RunRate;
                resultDto.OfflineRate = queryDto == null ? 0 : queryDto.OfflineRate;
                resultDto.DebugRate = queryDto == null ? 0 : queryDto.DebugRate;
            }

            return resultDto;
        }

        public async Task<IEnumerable<int>> GetPreviousMachineShiftDetailList(List<int> currentMachineShiftDetailList)
        {
            const string sql = @" WITH  tempTable  As 
              (
 	            SELECT ROW_NUMBER () OVER (PARTITION BY  msd.MachineId order by msd.ShiftDay,msd.ShiftSolutionItemId) AS SerialNum , msd.id, msd.MachineId ,msd.ShiftDay,msd.ShiftSolutionItemId
                FROM MachinesShiftDetails AS msd 
              ),
            currentSerialNum As 
            (
               SELECT t1.SerialNum,t1.machineId FROM tempTable As t1 where t1.id in @currentMachineShiftDetailList 
            )
             SELECT  t1.Id FROM  temptable  t1
             inner join currentSerialNum As c on (t1.machineid = c.machineid and t1.SerialNum + 1 = c.SerialNum) 
             ORDER BY t1.ShiftDay DESC ";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                return await conn.QueryAsync<int>(sql, new { currentMachineShiftDetailList });
            }
        }

        public async Task<List<MachineUsedTimeRateDto>> QueryMachineUsedTimeRate(IEnumerable<int> currentMachineShiftId)
        {
            var  returnValue =new List <MachineUsedTimeRateDto>();

            var executeSql = @"
SELECT B.SortSeq,
       B.Id AS MachineId,
       B.Name AS MachineName,
       ISNULL(A.Stop, 0) AS StopRate,
       ISNULL(A.Run, 0) AS RunRate,
       ISNULL(A.Free, 0) AS FreeRate,
       ISNULL(A.Offline, 0) AS OfflineRate,
       ISNULL(A.Debug, 0) AS DebugRate
FROM
(
    SELECT A.MachineId,
           A.Code,
           A.Rate
    FROM
    (
        SELECT A.MachineId,
               A.Code,
               ROUND(
                        CAST(SUM(A.Duration) OVER (PARTITION BY MachineId, A.Code) AS FLOAT)
                        * 100 / SUM(Duration) OVER (PARTITION BY MachineId),
                        2
                    ) AS Rate,
               ROW_NUMBER() OVER (PARTITION BY MachineId, A.Code ORDER BY A.MachineId, A.Code) CodeRow
        FROM
        (
             SELECT s.MachineId,
                   Code,
                   CASE
                       WHEN EndTime IS NULL THEN
                           DATEDIFF(SECOND, StartTime, GETDATE())
                       ELSE
                           [Duration]
                   END AS [Duration]
            FROM States s
            WHERE s.MachinesShiftDetailId IN @MachinesShiftDetailId
        ) AS A
    ) AS A
    WHERE A.CodeRow = 1
) AS A
PIVOT
(
    MAX(Rate)
    FOR [Code] IN ([Stop], [Run], [Free], [Offline], [Debug])
) AS A
    JOIN Machines AS B
        ON A.MachineId = B.Id;
";

            var parameters = new
            {
                MachinesShiftDetailId = currentMachineShiftId
            };

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();

                var queryResult = (await conn.QueryAsync<MachineUsedTimeRateDto>(executeSql, parameters)).OrderBy(s => s.SortSeq).ToList();

                for (var i = 0; i < queryResult.Count; i++)
                {
                    queryResult[i].OfflineRate = Math.Round(100 - queryResult[i].StopRate - queryResult[i].RunRate - queryResult[i].FreeRate - queryResult[i].DebugRate, 2);
                }

                returnValue = queryResult;
            }

            return returnValue;
        }
    }
}
