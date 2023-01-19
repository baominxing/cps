namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.StaffPerformace
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Configuration;
    using Abp.UI;
    using Dapper;
    using Wimi.BtlCore.CommonEnums;
    using Wimi.BtlCore.Configuration;
    using Wimi.BtlCore.StaffPerformance;

    public class StaffPerformaceRepository : IStaffPerformaceRepository
    {
        private readonly ISettingManager _settingManager;

        public StaffPerformaceRepository(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }
        public async Task<IEnumerable<dynamic>> CapacityQuery(IEnumerable<long> machineIds, DateTime startTime, DateTime endTime, List<string> unionTables)
        {
            var executeSql = $@"
SELECT ShiftDetail_SolutionName,
       UserId,
       MachineId,
       Yield,
       StartTime,
       ShiftDetail_StaffShiftName,
       ShiftDetail_MachineShiftName,
       ShiftDetail_ShiftDay
FROM [Capacities] WHERE ShiftDetail_ShiftDay BETWEEN @StartTime And @EndTime AND MachineId IN @MachineIds AND UserId <> 0 AND ShiftDetail_ShiftDay IS NOT NULL" + Environment.NewLine;

            foreach (var item in unionTables)
            {
                executeSql += $@"
UNION ALL
SELECT ShiftDetail_SolutionName,
       UserId,
       MachineId,
       Yield,
       StartTime,
       ShiftDetail_StaffShiftName,
       ShiftDetail_MachineShiftName,
       ShiftDetail_ShiftDay
FROM [{item}] WHERE ShiftDetail_ShiftDay BETWEEN @StartTime And @EndTime AND MachineId IN @MachineIds AND UserId <> 0 AND ShiftDetail_ShiftDay IS NOT NULL" + Environment.NewLine;
            }

            var sqlParameters = new { MachineIds = machineIds, StartTime = startTime, EndTime = endTime };

            var result = await this.Execute<dynamic>(executeSql, sqlParameters);

            return result;
        }

        /// <summary>
        ///     获取原因基础数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<StaffPerformanceState>> GetStaffReasonPerformance(StaffPerformance input)
        {
            var executeSql = await this.GenerateStaffReasonPerformanceSqlString(input);

            var parameters = new { input.StartTime, input.EndTime, input.MachineIdList, input.UserIdList };

            return await this.Execute<StaffPerformanceState>(executeSql, parameters);
        }

        /// <summary>
        ///     获取状态基础数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<StaffPerformanceState>> GetStaffStatePerformance(StaffPerformance input)
        {
            var executeSql = await this.GenerateStaffStatePerformanceSqlString(input);

            var parameters = new { input.StartTime, input.EndTime, input.MachineIdList, input.UserIdList, input.ShiftSolutionId };

            return await this.Execute<StaffPerformanceState>(executeSql, parameters);
        }

        /// <summary>
        /// 执行Dapper查询
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="executeSql">
        /// The execute Sql.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<IEnumerable<T>> Execute<T>(string executeSql, object parameters)
            where T : class
        {
            IEnumerable<T> result;

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();
                result = await conn.QueryAsync<T>(executeSql, parameters);
            }

            return result;
        }

        /// <summary>
        ///     获取原因SQL
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<string> GenerateStaffReasonPerformanceSqlString(StaffPerformance input)
        {
            EnumStatisticalWays enmuStatisticalWay;
            if (!Enum.TryParse(input.StatisticalWay, out enmuStatisticalWay))
                enmuStatisticalWay = EnumStatisticalWays.ByDay;

            var joinClause = string.Empty;
            var selectString = string.Empty;
            string whereString;
            var calendarJoinString = string.Empty;
            var orderbyString = "Order By MachineId,SummaryDate DESC";
            var machinesShiftDetailId = string.Empty;
            var selectedMachinesShiftDetailId = "NULL";
            switch (enmuStatisticalWay)
            {
                case EnumStatisticalWays.ByDay:
                    selectString = "Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23)";
                    whereString = input.UserIdListAllSelected()
                                      ? "WHERE  Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23) BETWEEN @StartTime AND @EndTime And s.MachineId IN @MachineIdList"
                                      : "WHERE  Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23) BETWEEN @StartTime AND @EndTime And s.MachineId IN @MachineIdList And s.UserId IN @UserIdList";
                    break;
                case EnumStatisticalWays.ByWeek:
                    var weekResult = await this.GetWeekSummaryDate(input);
                    var staffPerformanceStates = weekResult as StaffPerformanceState[] ?? weekResult.ToArray();
                    var staffPerformanceState = staffPerformanceStates.FirstOrDefault();

                    if (staffPerformanceState != null)
                    {
                        input.StartTime = Convert.ToDateTime(staffPerformanceState.SummaryDate);
                    }

                    var lastOrDefault = staffPerformanceStates.LastOrDefault();

                    if (lastOrDefault != null)
                    {
                        input.EndTime = Convert.ToDateTime(lastOrDefault.SummaryDate);
                    }

                    selectString = "c.YYYYISOWeek";
                    calendarJoinString =
                        "INNER JOIN Calendars AS c ON  c.Date = Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23)";
                    whereString = input.UserIdListAllSelected()
                                      ? "WHERE  c.[Date] BETWEEN @StartTime AND @EndTime And s.MachineId IN @MachineIdList"
                                      : "WHERE  c.[Date] BETWEEN @StartTime AND @EndTime And s.MachineId IN @MachineIdList And s.UserId IN @UserIdList";
                    break;
                case EnumStatisticalWays.ByMonth:
                    selectString = "c.YYYYMM";
                    calendarJoinString =
                        "INNER JOIN Calendars AS c ON  c.Date = Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23)";
                    whereString = input.UserIdListAllSelected()
                                      ? "WHERE  c.YYYYMM BETWEEN convert(nvarchar(7),@StartTime,23) AND convert(nvarchar(7),@EndTime,23) And s.MachineId IN @MachineIdList"
                                      : "WHERE  c.YYYYMM BETWEEN convert(nvarchar(7),@StartTime,23) AND convert(nvarchar(7),@EndTime,23) And s.MachineId IN @MachineIdList And s.UserId IN @UserIdList";
                    break;
                case EnumStatisticalWays.ByYear:
                    selectString = "c.Year";
                    calendarJoinString =
                        "INNER JOIN Calendars AS c ON  c.Date = Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23)";
                    whereString = input.UserIdListAllSelected()
                                      ? "WHERE  c.Year BETWEEN convert(nvarchar(4),@StartTime,23) AND convert(nvarchar(4),@EndTime,23) And s.MachineId IN @MachineIdList"
                                      : "WHERE  c.Year BETWEEN convert(nvarchar(4),@StartTime,23) AND convert(nvarchar(4),@EndTime,23) And s.MachineId IN @MachineIdList And s.UserId IN @UserIdList";
                    break;
                case EnumStatisticalWays.ByShift:
                    switch (input.QueryType)
                    {
                        case EnumStaffPerformanceQueryType.ByStaff:
                            selectString =
                                "Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23)+' '+s.ShiftDetail_StaffShiftName";
                            break;
                        case EnumStaffPerformanceQueryType.ByMachine:
                            selectString =
                                "Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23)+' '+s.ShiftDetail_MachineShiftName";
                            break;
                    }

                    whereString = input.UserIdListAllSelected()
                                      ? "WHERE  s.ShiftDetail_ShiftDay BETWEEN @StartTime AND @EndTime And s.MachineId IN @MachineIdList"
                                      : "WHERE  s.ShiftDetail_ShiftDay BETWEEN @StartTime AND @EndTime And s.MachineId IN @MachineIdList And s.UserId IN @UserIdList";
                    orderbyString = "Order By MachinesShiftDetailId";
                    machinesShiftDetailId = "MachinesShiftDetailId,";
                    selectedMachinesShiftDetailId = "C.MachinesShiftDetailId";
                    joinClause = "AND C.MachinesShiftDetailId = B.MachinesShiftDetailId";
                    break;
                default: throw new UserFriendlyException("方法未实现");
            }

            var executeSql = string.Format(
                @"
SELECT C.MachineId,
       m.Name            MachineName,
       C.UserId,
       u.Name UserName,
       C.SummaryDate,
       {0} MachinesShiftDetailId,
       B.Code            ReasonCode,
       B.DisplayName     ReasonName,
       B.Duration        ReasonDuration,
       B.Hexcode,
       C.TotalDuration
FROM   (
                SELECT MachineId,
                       UserId,
                       SummaryDate,
                       {1}
                       SUM(ISNULL(CAST(DATEDIFF(SECOND, StartTime, EndTime) AS BIGINT), 0)) AS  TotalDuration
                FROM   (
                           SELECT s.MachineId,
                                  s.UserId,
                                  {2} SummaryDate,
                                  {3}
                                  CASE 
                                       WHEN s.StartTime <= @StartTime THEN @StartTime
                                       ELSE s.StartTime
                                  END  AS StartTime,
                                  CASE 
                                       WHEN s.EndTime > DATEADD(DAY,1,@EndTime) THEN DATEADD(DAY,1,@EndTime)
                                       ELSE s.EndTime
                                  END  AS EndTime
                           FROM   STATEs s
                                  {4}
                                  INNER JOIN StateInfos AS sis
                                       ON  sis.Code = s.Code
                                       AND sis.Type = 0
                           {5}
                       ) A
                GROUP BY
                       MachineId,
                       UserId,
                       {6}
                       SummaryDate
            ) C
       LEFT JOIN (
           SELECT MachineId,
                  UserId,
                  SummaryDate,
                  Code,
                  DisplayName,
                  Hexcode,
                  {7}
                  SUM(ISNULL(CAST(DATEDIFF(SECOND, StartTime, EndTime) AS BIGINT), 0)) AS Duration
           FROM   (
                      SELECT s.MachineId,
                             s.UserId,
                             {8} SummaryDate,
                             CASE 
                                  WHEN s.StartTime <= @StartTime THEN @StartTime
                                  ELSE s.StartTime
                             END  AS StartTime,
                             CASE 
                                  WHEN s.EndTime > DATEADD(DAY,1,@EndTime) THEN DATEADD(DAY,1,@EndTime)
                                  ELSE s.EndTime
                             END  AS EndTime,
                             s.Code,
                             {9}
                             sis.DisplayName,
                             sis.Hexcode
                      FROM   STATEs s
                             INNER JOIN Calendars AS c
                                  ON  c.datekey = s.datekey
                             INNER JOIN StateInfos AS sis
                                  ON  sis.Code = s.Code
                                  AND sis.Type = 1
                      {10}
                  ) A
           GROUP BY
                  MachineId,
                  UserId,
                  SummaryDate,
                  Code,
                  {11}
                  DisplayName,
                  Hexcode
       ) B
            ON  C.MachineId = B.MachineId
            AND C.UserId = B.UserId
            AND C.SummaryDate = B.SummaryDate
            {12}
       JOIN MACHINEs m
            ON  m.id = b.MachineId
       JOIN Users     AS u
            ON  u.Id = b.UserId
{13}
",
                selectedMachinesShiftDetailId,
                machinesShiftDetailId,
                selectString,
                machinesShiftDetailId,
                calendarJoinString,
                whereString,
                machinesShiftDetailId,
                machinesShiftDetailId,
                selectString,
                machinesShiftDetailId,
                whereString,
                machinesShiftDetailId,
                joinClause,
                orderbyString);
            return executeSql;
        }

        /// <summary>
        ///     获取状态SQL
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<string> GenerateStaffStatePerformanceSqlString(StaffPerformance input)
        {
            EnumStatisticalWays enmuStatisticalWay;
            if (!Enum.TryParse(input.StatisticalWay, out enmuStatisticalWay))
                enmuStatisticalWay = EnumStatisticalWays.ByDay;

            var selectString = string.Empty;
            string whereString;
            var calendarJoinString = string.Empty;
            var orderbyString = "Order By MachineId,SummaryDate DESC";
            var machinesShiftDetailId = string.Empty;
            var selectedMachinesShiftDetailId = "NULL";
            var shiftCalendarsViewJoinString = string.Empty;
            switch (enmuStatisticalWay)
            {
                case EnumStatisticalWays.ByDay:
                    selectString = "Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23)";
                    whereString = input.UserIdListAllSelected()
                                      ? "WHERE  Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23) BETWEEN @StartTime AND @EndTime And s.MachineId IN @MachineIdList"
                                      : "WHERE  Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23) BETWEEN @StartTime AND @EndTime And s.MachineId IN @MachineIdList And s.UserId IN @UserIdList";
                    break;
                case EnumStatisticalWays.ByWeek:
                    var weekResult = await this.GetWeekSummaryDate(input);
                    var staffPerformanceStates = weekResult as StaffPerformanceState[] ?? weekResult.ToArray();
                    var staffPerformanceState = staffPerformanceStates.FirstOrDefault();
                    if (staffPerformanceState != null)
                    {
                        input.StartTime = Convert.ToDateTime(staffPerformanceState.SummaryDate);
                    }

                    var lastOrDefault = staffPerformanceStates.LastOrDefault();
                    if (lastOrDefault != null)
                    {
                        input.EndTime = Convert.ToDateTime(lastOrDefault.SummaryDate);
                    }

                    selectString = "c.YYYYISOWeek";
                    calendarJoinString =
                        "INNER JOIN Calendars AS c ON  c.Date = Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23)";
                    whereString = input.UserIdListAllSelected()
                                      ? "WHERE  c.[Date] BETWEEN @StartTime AND @EndTime And s.MachineId IN @MachineIdList"
                                      : "WHERE  c.[Date] BETWEEN @StartTime AND @EndTime And s.MachineId IN @MachineIdList And s.UserId IN @UserIdList";
                    break;
                case EnumStatisticalWays.ByMonth:
                    selectString = "c.YYYYMM";
                    calendarJoinString =
                        "INNER JOIN Calendars AS c ON  c.Date = Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23)";
                    whereString = input.UserIdListAllSelected()
                                      ? "WHERE  c.YYYYMM BETWEEN convert(nvarchar(7),@StartTime,23) AND convert(nvarchar(7),@EndTime,23) And s.MachineId IN @MachineIdList"
                                      : "WHERE  c.YYYYMM BETWEEN convert(nvarchar(7),@StartTime,23) AND convert(nvarchar(7),@EndTime,23) And s.MachineId IN @MachineIdList And s.UserId IN @UserIdList";
                    break;
                case EnumStatisticalWays.ByYear:
                    selectString = "c.Year";
                    calendarJoinString =
                        "INNER JOIN Calendars AS c ON  c.Date = Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23)";
                    whereString = input.UserIdListAllSelected()
                                      ? "WHERE  c.Year BETWEEN convert(nvarchar(4),@StartTime,23) AND convert(nvarchar(4),@EndTime,23) And s.MachineId IN @MachineIdList"
                                      : "WHERE  c.Year BETWEEN convert(nvarchar(4),@StartTime,23) AND convert(nvarchar(4),@EndTime,23) And s.MachineId IN @MachineIdList And s.UserId IN @UserIdList";
                    break;
                case EnumStatisticalWays.ByShift:
                    switch (input.QueryType)
                    {
                        case EnumStaffPerformanceQueryType.ByStaff:
                            selectString =
                                "Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23)+' '+s.ShiftDetail_StaffShiftName";
                            orderbyString = "Order By MachinesShiftDetailId";
                            machinesShiftDetailId = "MachinesShiftDetailId,";
                            selectedMachinesShiftDetailId = "MachinesShiftDetailId";
                            break;
                        case EnumStaffPerformanceQueryType.ByMachine:
                            selectString =
                                "Convert(nvarchar(10),s.ShiftDetail_ShiftDay,23)+' '+s.ShiftDetail_MachineShiftName";
                            orderbyString = "Order By MachinesShiftDetailId";
                            machinesShiftDetailId = "MachinesShiftDetailId,";
                            selectedMachinesShiftDetailId = "MachinesShiftDetailId";
                            break;
                    }

                    shiftCalendarsViewJoinString = "JOIN ShiftCalendarsView as scv ON s.MachinesShiftDetailId = scv.MachineShiftDetailId";

                    whereString = input.UserIdListAllSelected()
                                      ? "WHERE  s.ShiftDetail_ShiftDay BETWEEN @StartTime AND @EndTime And s.MachineId IN @MachineIdList AND s.ShiftDetail_StaffShiftName IS NOT NULL AND scv.ShiftSolutionId = @ShiftSolutionId"
                                      : "WHERE  s.ShiftDetail_ShiftDay BETWEEN @StartTime AND @EndTime And s.MachineId IN @MachineIdList AND s.UserId IN @UserIdList AND s.ShiftDetail_StaffShiftName IS NOT NULL  AND scv.ShiftSolutionId = @ShiftSolutionId";

                    break;
                default: throw new UserFriendlyException("方法未实现");
            }
            var unionQuery = $@"SELECT ShiftDetail_ShiftDay,MachineId,UserId,ShiftDetail_StaffShiftName,ShiftDetail_MachineShiftName,StartTime,EndTime,Code,MachinesShiftDetailId FROM dbo.States WHERE MachineId IN @MachineIdList" + Environment.NewLine;

            foreach (var item in input.UnionTables)
            {
                //查询的数据涉及到分表
                unionQuery += $@"UNION ALL SELECT ShiftDetail_ShiftDay,MachineId,UserId,ShiftDetail_StaffShiftName,ShiftDetail_MachineShiftName,StartTime,EndTime,Code,MachinesShiftDetailId FROM dbo.[{item}] WHERE MachineId IN @MachineIdList" + Environment.NewLine;
            }

            var executeSql = $@"
SELECT B.MachineId,
       m.Name                     MachineName,
       b.UserId,
       u.Name         UserName,
       B.SummaryDate,
       SUM(B.DebugDuration)       DebugDuration,
       SUM(B.OfflineDuration)     OfflineDuration,
       SUM(B.FreeDuration)        FreeDuration,
       SUM(B.RunDuration)         RunDuration,
       SUM(B.StopDuration)        StopDuration,
       SUM(B.TotalDuration)       TotalDuration,
       {selectedMachinesShiftDetailId} MachinesShiftDetailId,
       0                          IsByShift,
       REPLACE(B.SummaryDate, '-', '') ShiftDay
FROM   (
           SELECT MachineId,
                  UserId,
                  SummaryDate,
                  CASE 
                       WHEN Code = 'Debug' THEN ISNULL(DATEDIFF(SECOND, StartTime, EndTime), 0)
                       ELSE 0
                  END  AS DebugDuration,
                  CASE 
                       WHEN Code = 'Offline' THEN ISNULL(DATEDIFF(SECOND, StartTime, EndTime), 0)
                       ELSE 0
                  END  AS OfflineDuration,
                  CASE 
                       WHEN Code = 'Free' THEN ISNULL(DATEDIFF(SECOND, StartTime, EndTime), 0)
                       ELSE 0
                  END  AS FreeDuration,
                  CASE 
                       WHEN Code = 'Run' THEN ISNULL(DATEDIFF(SECOND, StartTime, EndTime), 0)
                       ELSE 0
                  END  AS RunDuration,
                  CASE 
                       WHEN Code = 'Stop' THEN ISNULL(DATEDIFF(SECOND, StartTime, EndTime), 0)
                       ELSE 0
                  END  AS StopDuration,
                  {machinesShiftDetailId}
                  ISNULL(DATEDIFF(SECOND, StartTime, EndTime), 0) AS TotalDuration
           FROM   (
                      SELECT s.MachineId,
                             s.UserId,
                             {selectString} SummaryDate,
                             s.StartTime,
                             ISNULL(s.EndTime,GETDATE()) AS EndTime,
                            {machinesShiftDetailId}
                             s.Code
                      FROM  ({unionQuery}) s
                      {shiftCalendarsViewJoinString}
                      {calendarJoinString}
                      INNER JOIN StateInfos As sis ON sis.Code = s.Code And sis.Type = 0
                      {whereString}
                  ) A
       ) B
       JOIN MACHINEs m
            ON  m.id = b.MachineId
       JOIN Users AS u
            ON  u.Id = b.UserId
GROUP BY
       MachineId,
       m.Name,
       UserId,
       u.Name,
       {machinesShiftDetailId}
       SummaryDate
{orderbyString}
";
            return executeSql;
        }

        /// <summary>
        ///     修正选择周的时间范围
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<IEnumerable<StaffPerformanceState>> GetWeekSummaryDate(StaffPerformance input)
        {
            var executeSql = $@"
SELECT c.[Date] SummaryDate
FROM   Calendars AS c
WHERE  c.YYYYISOWeek IN (SELECT DISTINCT c2.YYYYISOWeek
                         FROM   Calendars AS c2
                         WHERE  c2.[Date] BETWEEN @StartTime AND @EndTime)";

            var sqlParameters = new { input.StartTime, input.EndTime };

            return await this.Execute<StaffPerformanceState>(executeSql, sqlParameters);
        }
    }
}