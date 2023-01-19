using Abp.Domain.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.BasicData.Shifts.Manager.Dto;
using Wimi.BtlCore.CommonEnums;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.BasicData.Shifts.Manager
{
    public class ShiftCalendarManager : BtlCoreDomainServiceBase, IShiftCalendarManager
    {
        private readonly IRepository<ShiftCalendar, long> shiftcalendarRepository;
        private readonly DeviceGroupManager deviceGroupManager;
        private readonly MachineManager machineManager;

        public ShiftCalendarManager(
            IRepository<ShiftCalendar, long> shiftcalendarRepository,
            DeviceGroupManager deviceGroupManager,
            MachineManager machineManager
            )
        {
            this.shiftcalendarRepository = shiftcalendarRepository;
            this.deviceGroupManager = deviceGroupManager;
            this.machineManager = machineManager;
        }
        /// <summary>
        /// 获取设备列表关联的班次方案列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GetMachineShiftSolutionsDto>> GetMachineShiftSolutions(DateTime startTime, DateTime endTime, List<int> machineIdList)
        {
            var executeSql = $@"
SELECT 
ShiftSolutionId MachineShiftSolutionId,
ShiftSolutionName MachineShiftSolutionName,  
MachineId,
MachineName 
FROM ShiftCalendarsView WHERE ShiftDay BETWEEN @StartTime AND @EndTime AND MachineId IN @MachineIdList
GROUP BY
ShiftSolutionId,
ShiftSolutionName,
MachineId,
MachineName
";
            var param = new { StartTime = startTime, EndTime = endTime, MachineIdList = machineIdList };
            return await this.ExecuteForData<GetMachineShiftSolutionsDto>(executeSql, param);
        }

        /// <summary>
        /// 获取设备组列表关联的班次方案列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GetMachineShiftSolutionsDto>> GetMachineGroupShiftSolutions(DateTime startTime, DateTime endTime, List<int> machineGroupIdList)
        {
            //根据设备组下关联的设备进行筛选
            var allmachines = await this.machineManager.GetInDeviceGroupMachines();
            var filteredmachines = allmachines.Where(s => machineGroupIdList.Contains(s.DeviceGroupId));

            return await this.GetMachineShiftSolutions(startTime, endTime, filteredmachines.Select(s => s.Id).ToArray().ToList());
        }

        /// <summary>
        /// 根据日历表获取所选日期的时间范围
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GetSummaryDateDto>> GetSummaryDate(DateTime startTime, DateTime endTime, EnumStatisticalWays statisticalWay)
        {
            string showColumn = string.Empty;
            string groupBy = string.Empty;
            string orderBy = string.Empty;

            switch (statisticalWay)
            {
                case EnumStatisticalWays.ByShift:
                case EnumStatisticalWays.ByDay:
                    showColumn = "DATE SummaryDate";
                    orderBy = "Order By DATE ";
                    break;
                case EnumStatisticalWays.ByWeek:
                    showColumn = "CONVERT(NVARCHAR,Year)+ '-'+ CONVERT(NVARCHAR,ISOWeekOfYear) SummaryDate";
                    groupBy = "GROUP BY CONVERT(NVARCHAR,Year)+ '-'+ CONVERT(NVARCHAR,ISOWeekOfYear)";
                    orderBy = "Order By CONVERT(NVARCHAR,Year)+ '-'+ CONVERT(NVARCHAR,ISOWeekOfYear)";
                    break;
                case EnumStatisticalWays.ByMonth:
                    showColumn = "CONVERT(NVARCHAR,Year)+ '-'+ CONVERT(NVARCHAR,MONTH) SummaryDate";
                    groupBy = "GROUP BY CONVERT(NVARCHAR,Year)+ '-'+ CONVERT(NVARCHAR,MONTH)";
                    orderBy = "Order By CONVERT(NVARCHAR,Year)+ '-'+ CONVERT(NVARCHAR,MONTH)";
                    break;
                case EnumStatisticalWays.ByYear:
                    showColumn = "Year SummaryDate";
                    groupBy = "GROUP BY Year";
                    orderBy = "Order By Year";
                    break;
            }

            string executeSql = $@"SELECT {showColumn}  FROM calendars WHERE DATE >=@StartTime AND DATE <=@EndTime {groupBy} {orderBy}";

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();
                var sqlParameters = new { StartTime = startTime, EndTime = endTime };

                return await conn.QueryAsync<GetSummaryDateDto>(executeSql, sqlParameters);
            }
        }

        /// <summary>
        /// 根据传入时间范围，计算按周，按月，按年统计需要的正确的开始时间和结束时间
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<CorrectQueryDateDto>> CorrectQueryDate(DateTime startTime, DateTime endTime, EnumStatisticalWays statisticalWay, List<int> machineIdList, List<int> shiftSolutionIdList)
        {
            var shiftCalendarList = new List<CorrectQueryDateDto>();
            var showColumn = string.Empty;
            var groupBy = string.Empty;
            var orderBy = string.Empty;

            switch (statisticalWay)
            {
                case EnumStatisticalWays.ByMachineShiftDetail:
                case EnumStatisticalWays.ByShift:
                case EnumStatisticalWays.ByDay:
                    showColumn = "CONVERT(NVARCHAR(10), DATE, 120) ShiftDay";
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

            string executeSql = $@"SELECT {showColumn}  FROM calendars WHERE DATE >=@StartTime AND DATE <=@EndTime {groupBy} {orderBy}";

            var correctedQueryDateList = new List<dynamic>();

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();
                var sqlParameters = new { StartTime = startTime, EndTime = endTime };

                correctedQueryDateList = conn.Query<dynamic>(executeSql, sqlParameters).ToList();
            }

            shiftCalendarList = (await this.GetShiftCalendars(correctedQueryDateList, statisticalWay, machineIdList, shiftSolutionIdList)).OrderBy(x => x.StartTime).ToList();

            return shiftCalendarList;
        }

        /// <summary>
        /// 依据所选统计方式和日期调整正确的日期范围
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<List<CorrectQueryDateDto>> GetShiftCalendars(IEnumerable<dynamic> correctedQueryDateList, EnumStatisticalWays statisticalWay, List<int> machineIdList, List<int> shiftSolutionIdList)
        {
            var shiftCalendarList = new List<CorrectQueryDateDto>();

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
                case EnumStatisticalWays.ByMachineShiftDetail:
                case EnumStatisticalWays.ByShift:
                    shiftIdColumn = @"Convert(NVARCHAR,sc.MachineShiftDetailId)";
                    startTimeColumn = "A.StartTime";
                    endTimeColumn = @"CASE WHEN A.EndTime > GETDATE() THEN GETDATE() ELSE a.EndTime END";
                    whereClause = $"c.DATE >= '{correctedQueryDateList.FirstOrDefault().ShiftDay}' AND c.DATE <= '{correctedQueryDateList.LastOrDefault().ShiftDay}' AND sc.ShiftSolutionId IN @ShiftSolutionIdList";
                    groupByColumn = string.Empty;
                    break;
                case EnumStatisticalWays.ByDay:
                    whereClause = $"c.DATE >= '{correctedQueryDateList.FirstOrDefault().ShiftDay}' AND c.DATE <= '{correctedQueryDateList.LastOrDefault().ShiftDay}'";
                    break;
                case EnumStatisticalWays.ByWeek:
                    whereClause = $"c.YYYYISOWeek >= '{correctedQueryDateList.FirstOrDefault().ShiftWeek}' AND c.YYYYISOWeek <= '{correctedQueryDateList.LastOrDefault().ShiftWeek}'";
                    break;
                case EnumStatisticalWays.ByMonth:
                    whereClause = $"c.YYYYMM >= '{correctedQueryDateList.FirstOrDefault().ShiftMonth}' AND c.YYYYMM <= '{correctedQueryDateList.LastOrDefault().ShiftMonth}'";
                    break;
                case EnumStatisticalWays.ByYear:
                    whereClause = $"c.Year >= '{correctedQueryDateList.FirstOrDefault().ShiftYear}' AND c.Year <= '{correctedQueryDateList.LastOrDefault().ShiftYear}'";
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
                  {shiftIdColumn}                               ShiftId,
                  CONVERT(NVARCHAR(10), c.[Date], 120)          ShiftDay,
                  c.YYYYISOWeek                                 ShiftWeek,
                  c.YYYYMM                                      ShiftMonth,
                  c.[Year]                                      ShiftYear,
                  sc.StartTime,
                  sc.EndTime
           FROM Calendars AS c
           JOIN ShiftCalendars AS sc 
           ON sc.ShiftDay = c.[Date] 
           WHERE {whereClause}
           AND sc.MachineId IN @MachineIdList
           AND sc.StartTime <= GETDATE()
       )   AS A
{groupByColumn}
";
            var parameters = new { MachineIdList = machineIdList, ShiftSolutionIdList = shiftSolutionIdList };

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();
                return (await conn.QueryAsync<CorrectQueryDateDto>(executeSql, parameters)).ToList();
            }
        }

        private async Task<IEnumerable<T>> ExecuteForData<T>(string executeSql, object parameters)
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
    }
}
