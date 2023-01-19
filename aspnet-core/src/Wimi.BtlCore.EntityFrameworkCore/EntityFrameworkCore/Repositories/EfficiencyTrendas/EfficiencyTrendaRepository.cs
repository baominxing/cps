using Abp.Domain.Repositories;
using Castle.Core.Logging;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.BasicData.Shifts.Manager;
using Wimi.BtlCore.CommonEnums;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.EfficiencyTrendas;
using Wimi.BtlCore.EfficiencyTrendas.Dtos;

namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.EfficiencyTrendas
{
    public class EfficiencyTrendaRepository : IEfficiencyTrendaRepository
    {
        public ILogger Logger;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IShiftCalendarManager shiftCalendarManager;
        private readonly IRepository<ShiftSolution> shiftSolutionRepository;

        public EfficiencyTrendaRepository(IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
             IShiftCalendarManager shiftCalendarManager,
             IRepository<ShiftSolution> shiftSolutionRepository)
        {
            this.Logger = NullLogger.Instance;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.shiftCalendarManager = shiftCalendarManager;
            this.shiftSolutionRepository = shiftSolutionRepository;
        }

        /// <summary>
        ///     如果当前查询涉及当天数据，首先运行存储过程得到最新数据的summary
        /// </summary>
        /// <param name="machineIdList"></param>
        /// <returns></returns>
        public async Task GetCurrentStateDurationStatistics(string machineIdList)
        {
            try
            {
                using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
                {
                    conn.Open();
                    await conn.ExecuteAsync(
                        "sp_CurrentStateDurationStatistics",
                        new { MachineIdList = machineIdList },
                        commandType: CommandType.StoredProcedure,
                        commandTimeout: 120);
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error($@"方法GetCurrentStateDurationStatistics执行失败,原因:{ex.Message}");
            }
        }

        public List<EfficiencyTrendasDataTablesDto> GetEfficiencyTrendasDataTablesColumns(EfficiencyTrendsInputDto input)
        {
            EnumStatisticalWays enmuStatisticalWays;
            if (!Enum.TryParse(input.StatisticalWays, out enmuStatisticalWays))
                enmuStatisticalWays = EnumStatisticalWays.ByDay;
            if (enmuStatisticalWays == EnumStatisticalWays.ByShift)
            {
                var sql = string.Empty;
                if (input.QueryType == "1")
                {
                    sql = $@"SELECT Distinct Convert(varchar, mdg.DeviceGroupId)+'$'+d.DisplayName 
                             FROM ShiftCalendarsView as s
                             join MachineDeviceGroups AS mdg on s.MachineId = mdg.MachineId
                             join DeviceGroups AS d on mdg.DeviceGroupId = d.Id
                             WHERE s.ShiftDay Between @StartTime And  @EndTime
                             AND s.ShiftSolutionName in @MachineShiftSolutionNameList
                             AND mdg.DeviceGroupId in @MachineId";
                }
                else
                {
                    sql = $@"SELECT Distinct Convert(varchar,MachineId)+'$'+MachineName 
                             FROM ShiftCalendarsView
                             WHERE ShiftDay Between @StartTime And @EndTime
                             AND ShiftSolutionName in @MachineShiftSolutionNameList
                             AND MachineId in @MachineId";
                }
                using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
                {
                    conn.Open();
                    var result = conn.Query<string>(sql, new { input.StartTime, input.EndTime, input.MachineShiftSolutionNameList, input.MachineId }).ToList();
                    input.MachineName = string.Join(",", result.ToArray());
                }
            }

            var columnList = new List<EfficiencyTrendasDataTablesDto>();
            if (!string.IsNullOrWhiteSpace(input.MachineName))
            {
                var count = 0;
                foreach (var item in input.MachineName.Split(','))
                {
                    count++;
                    var target = item.Split('$');
                    columnList.Add(
                        new EfficiencyTrendasDataTablesDto
                        {
                            Data = target.FirstOrDefault(),
                            Title = target.LastOrDefault(),
                            ClassName = count > 4 ? "not-mobile" : string.Empty
                        });
                }

                return columnList;
            }

            return null;
        }

        public async Task<List<ExpandoObject>> GetEfficiencyTrendasExpandoObject(EnumStatisticalWays enmuStatisticalWays, List<int> machineId,
            List<int> machineShiftDetailId, List<string> machineShiftSolutionNameList, string queryType, DateTime startTime, DateTime endTime,List<string> unionTables)
        {
            var statisticalWaysSql = string.Empty;
            var selectWaysSql = string.Empty;
            var groupBySql = string.Empty;
            var where = string.Empty;
            var shiftWhereAnd = string.Empty;

            var shiftIdList = new List<int>();

            switch (enmuStatisticalWays)
            {
                case EnumStatisticalWays.ByDay:
                    statisticalWaysSql = " sc.ShiftDayName";
                    selectWaysSql = " sc.ShiftDayName";
                    break;
                case EnumStatisticalWays.ByWeek:
                    statisticalWaysSql = " sc.ShiftWeekName";
                    selectWaysSql = "sc.ShiftWeekName";
                    break;
                case EnumStatisticalWays.ByMonth:
                    statisticalWaysSql = " sc.ShiftMonthName";
                    selectWaysSql = " sc.ShiftMonthName";
                    break;
                case EnumStatisticalWays.ByYear:
                    selectWaysSql = statisticalWaysSql = " sc.ShiftYearName";
                    break;
                case EnumStatisticalWays.ByShift:
                    selectWaysSql = " sc.ShiftItemId,sc.ShiftItemName,sc.ShiftDayName";
                    statisticalWaysSql = " sc.ShiftDayName,sc.ShiftItemId,sc.ShiftItemName";
                    where = machineShiftDetailId == null ? "AND 1=1 " : "AND st.MachinesShiftDetailId IN @MachineShiftDetailIdList";
                    shiftWhereAnd = "AND sc.ShiftSolutionName in @ShiftSolutionName";

                    shiftIdList = this.shiftSolutionRepository.GetAll().Where(x => machineShiftSolutionNameList.Contains(x.Name)).Select(x => x.Id).ToList();
                    break;
            }

            var pivotString = string.Empty;
            var idString = string.Empty;

            foreach (var item in machineId)
            {
                pivotString += $@"[{item}],";
                idString += $@"{item},";
            }

            pivotString = pivotString.Trim(',');
            idString = idString.Trim(',');

            string groupId;
            string selectGroupId;
            var machineIdList = new List<int>();

            switch (queryType)
            {
                case "0":
                    groupId = "st.MachineId";
                    selectGroupId = groupId;
                    machineIdList = machineId;
                    break;
                default:
                    groupId = "mdg.DeviceGroupId";
                    selectGroupId = "mdg.DeviceGroupId as MachineId";
                    machineIdList = this.machineDeviceGroupRepository.GetAll().Where(x => machineId.Contains(x.DeviceGroupId)).Select(x => x.MachineId).ToList();
                    break;
            }

            var unionQuery = $@"SELECT * FROM dbo.States WHERE MachineId IN ({string.Join(",", machineIdList)})" + Environment.NewLine;

            foreach (var item in unionTables)
            {
                //查询的数据涉及到分表
                unionQuery += $@"UNION ALL SELECT * FROM dbo.[{item}] WHERE MachineId IN ({string.Join(",", machineIdList)})" + Environment.NewLine;
            }

            var sql = $@"SELECT piv.* FROM (
                            SELECT {selectGroupId}, {selectWaysSql} as dimensions,
                            CONVERT(
                                          DECIMAL(10, 2),
                                           CAST(SUM(CASE WHEN st.Code = 'Run' THEN st.Duration ELSE 0 END) AS FLOAT)
                                          /
                                          CASE WHEN SUM(st.Duration) = 0 THEN 1 ELSE ISNULL(SUM(st.Duration),1) END 
                                          *
                                          100
                                      )                         Rate
                              FROM  ({unionQuery})   AS st
                              INNER JOIN ShiftCalendarsView AS sc ON sc.MachineShiftDetailId = st.MachinesShiftDetailId
                              INNER JOIN [Machines] AS m ON m.Id=st.MachineId 
                              INNER JOIN MachineDeviceGroups AS mdg ON (mdg.MachineId=m.Id)
                        WHERE sc.shiftDay BETWEEN @StartTime AND @EndTime 
                        {where}
                        AND {groupId} IN @MachineIdList
                        {shiftWhereAnd}
                        GROUP BY {groupId}, {statisticalWaysSql} ) AS T 
                        PIVOT(MAX(T.Rate) FOR T.MachineId IN ({pivotString})) AS piv
                        order by piv. dimensions";

            if (enmuStatisticalWays == EnumStatisticalWays.ByShift)
            {
                sql = sql += ",piv.ShiftItemId";
            }

            if (endTime >= DateTime.Today)
            {
                await this.GetCurrentStateDurationStatistics(idString);
            }

            var formartData = await this.shiftCalendarManager.CorrectQueryDate(startTime, endTime, enmuStatisticalWays, machineIdList, shiftIdList);

            var result = new List<ExpandoObject>();

            using (var conn = new SqlConnection(AppSettings.Database.ConnectionString))
            {
                conn.Open();
                var param = new
                {
                    StartTime = formartData.Count() == 0 ? startTime : Convert.ToDateTime(formartData.First().ShiftDay),
                    EndTime = formartData.Count() == 0 ? endTime : Convert.ToDateTime(formartData.Last().ShiftDay),
                    EndTimeAdd = formartData.Count() == 0 ? endTime.AddDays(2) : Convert.ToDateTime(formartData.Last().ShiftDay).AddDays(2),
                    MachineIdList = machineId,
                    MachineShiftDetailIdList = machineShiftDetailId,
                    ShiftSolutionName = machineShiftSolutionNameList
                };

                result = conn.Query(sql, param).Select(x => (ExpandoObject)ToExpandoDynamic(x)).ToList();

            }

            return result;
        }

        private static dynamic ToExpandoDynamic(object value)
        {
            var dapperRowProperties = value as IDictionary<string, object>;
            IDictionary<string, object> expando = new ExpandoObject();

            if (dapperRowProperties != null)
                foreach (var property in dapperRowProperties) expando.Add(property.Key, property.Value);

            return expando as ExpandoObject;
        }
    }
}