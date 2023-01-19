using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Castle.Core.Logging;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines.Repository;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.BasicData.Shifts.Manager;
using Wimi.BtlCore.CommonEnums;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.EfficiencyTrendas.Dtos;

namespace Wimi.BtlCore.EntityFrameworkCore.Repositories.Machines
{
    public class ActivationRepository : IActivationRepository
    {
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<ShiftSolution> shiftSolutionRepository;
        private readonly IShiftCalendarManager shiftCalendarManager;
        private readonly ISettingManager settingManager;
        public ILogger Logger;
        private readonly string connectionString;

        public ActivationRepository(
                IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
                IRepository<ShiftSolution> shiftSolutionRepository,
                IShiftCalendarManager shiftCalendarManager,
                ISettingManager settingManager
            )
        {
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.shiftSolutionRepository = shiftSolutionRepository;
            this.shiftCalendarManager = shiftCalendarManager;
            this.settingManager = settingManager;
            this.connectionString = AppSettings.Database.ConnectionString;
            this.Logger = NullLogger.Instance;
        }


        [AbpAllowAnonymous]
        public async Task<List<ExpandoObject>> GetMachineActivationOriginalData(EfficiencyTrendsInputDto input)
        {
            if (input.MachineId.Count == 0) return new List<ExpandoObject>();

            EnumStatisticalWays enmuStatisticalWays;
            if (!Enum.TryParse(input.StatisticalWays, out enmuStatisticalWays))
                enmuStatisticalWays = EnumStatisticalWays.ByDay;

            var statisticalWaysSql = string.Empty;
            var selectWaysSql = string.Empty;
            var groupBySql = string.Empty;
            var shiftJoin = string.Empty;
            var shiftWhere = string.Empty;
            var where = "s.ShiftDetail_ShiftDay BETWEEN @StartTime AND @EndTime";

            var shiftIdList = new List<int>();

            switch (enmuStatisticalWays)
            {
                case EnumStatisticalWays.ByDay:
                    statisticalWaysSql = " sc.ShiftDayName";
                    selectWaysSql = "  sc.ShiftDayName";
                    break;
                case EnumStatisticalWays.ByWeek:
                    statisticalWaysSql = " sc.ShiftWeekName";
                    selectWaysSql = " sc.ShiftWeekName";
                    break;
                case EnumStatisticalWays.ByMonth:
                    statisticalWaysSql = " sc.ShiftMonthName";
                    selectWaysSql = " sc.ShiftMonthName";
                    break;
                case EnumStatisticalWays.ByYear:
                    selectWaysSql = statisticalWaysSql = " sc.ShiftYearName";
                    break;
                case EnumStatisticalWays.ByShift:
                    selectWaysSql = " sc.ShiftItemId ,sc.ShiftItemName, sc.ShiftDayName";
                    statisticalWaysSql = " sc.ShiftDayName,sc.ShiftItemId,sc.ShiftItemName";
                    where = input.MachineShiftDetailId == null ? where : "s.MachinesShiftDetailId IN @MachineShiftDetailIdList";
                    shiftWhere = "AND sc.ShiftSolutionName in @ShiftSolutionName";
                    shiftIdList = this.shiftSolutionRepository.GetAll().Where(x => input.MachineShiftSolutionNameList.Contains(x.Name)).Select(x => x.Id).ToList();
                    break;
                case EnumStatisticalWays.ByMachineShiftDetail:
                    selectWaysSql = " sc.ShiftItemId ,sc.ShiftItemName, sc.ShiftDayName";
                    statisticalWaysSql = " sc.ShiftDayName,sc.ShiftItemId,sc.ShiftItemName";
                    where = input.MachineShiftDetailId == null ? where : "s.MachinesShiftDetailId IN @MachineShiftDetailIdList";
                    break;
            }

            var pivotString = string.Empty;

            foreach (var item in input.MachineId) pivotString += $@"[{item}],";

            pivotString = pivotString.Trim(',');

            string groupId;
            string selectGroupId;

            var machineIdList = new List<int>();

            switch (input.QueryType)
            {
                case "0":
                    groupId = "s.MachineId";
                    selectGroupId = groupId;
                    machineIdList = input.MachineId;
                    break;
                default:
                    groupId = "mdg.DeviceGroupId";
                    selectGroupId = "mdg.DeviceGroupId as MachineId";
                    machineIdList = this.machineDeviceGroupRepository.GetAll().Where(x => input.MachineId.Contains(x.DeviceGroupId)).Select(x => x.MachineId).ToList();
                    break;
            }

            var unionQuery = $@"SELECT s.MachineId, s.Code, s.MachinesShiftDetailId,s.StartTime,s.EndTime,s.ShiftDetail_ShiftDay FROM dbo.States s WHERE MachineId IN ({string.Join(",", machineIdList)}) AND {where}" + Environment.NewLine;

            foreach (var item in input.UnionTables)
            {
                //查询的数据涉及到分表
                unionQuery += $@"UNION ALL SELECT s.MachineId, s.Code, s.MachinesShiftDetailId,s.StartTime,s.EndTime,s.ShiftDetail_ShiftDay FROM dbo.[{item}] s WHERE MachineId IN ({string.Join(",", machineIdList)}) AND {where}" + Environment.NewLine;
            }

            var sql = $@"
SELECT * FROM(SELECT piv.*
FROM (
SELECT 
{selectGroupId},
{selectWaysSql} as Dimensions, 
CONVERT(DECIMAL(10, 2),CAST(SUM(CASE WHEN si.IsPlaned = 1 THEN s.Duration ELSE 0 END) AS FLOAT)/CASE WHEN SUM(s.Duration) = 0 THEN 1 ELSE ISNULL(SUM(s.Duration),1) END * 100) Rate
FROM   
(
                                        SELECT s.MachineId,s.Code,SUM(DATEDIFF(SECOND, s.StartTime, CASE WHEN s.EndTime IS NULL THEN GETDATE() ELSE s.EndTime END)) AS Duration,s.MachinesShiftDetailId
                                        FROM ({unionQuery}) AS s
                                        INNER JOIN ShiftCalendarsView AS sc ON s.MachinesShiftDetailId = sc.MachineShiftDetailId 
                                        WHERE {where} {shiftWhere}
                                        GROUP BY s.MachineId, s.Code, s.MachinesShiftDetailId
                                       ) s
                                      INNER JOIN ShiftCalendarsView AS sc ON s.MachinesShiftDetailId= sc.MachineShiftDetailId 
                                      INNER JOIN StateInfos  AS si ON  si.Code = s.Code
                                      INNER JOIN [Machines]  AS m ON  m.Id = s.MachineId
                                      INNER JOIN MachineDeviceGroups AS mdg ON  (mdg.MachineId = m.Id)
                                      WHERE {groupId} IN @MachineIdList
                               GROUP BY
                                      {groupId},
                                      {statisticalWaysSql}
                           ) AS st
                           PIVOT(MAX(st.Rate) FOR st.MachineId IN ({pivotString})) AS piv)
                           AS tt  Order By tt.Dimensions"
             ;


            var formartData = await this.shiftCalendarManager.CorrectQueryDate(input.StartTime, input.EndTime, enmuStatisticalWays, machineIdList, shiftIdList);

            using (var conn = new SqlConnection(this.connectionString))
            {
                conn.Open();

                var param = new
                {
                    StartTime = formartData.Count() == 0 ? input.StartTime : Convert.ToDateTime(formartData.First().ShiftDay),
                    EndTime = formartData.Count() == 0 ? input.EndTime : Convert.ToDateTime(formartData.Last().ShiftDay),
                    EndTimeAdd = formartData.Count() == 0 ? input.EndTime.AddDays(2) : Convert.ToDateTime(formartData.Last().ShiftDay).AddDays(2),
                    MachineIdList = input.MachineId,
                    MachineShiftDetailIdList = input.MachineShiftDetailId,
                    ShiftSolutionName = input.MachineShiftSolutionNameList
                };

                sql = sql.Replace("@StartTime", $@"'{param.StartTime.ToString("yyyy-MM-dd HH:mm:ss")}'")
                   .Replace("@ExtendEndTime", $@"'{param.EndTimeAdd.ToString("yyyy-MM-dd HH:mm:ss")}'")
                   .Replace("@EndTime", $@"'{param.EndTime.ToString("yyyy-MM-dd HH:mm:ss")}'")
                   .Replace("@MachineIdList", $@"({param.MachineIdList?.JoinAsString(",")})")
                   .Replace("@MachineShiftDetailIdList", $@"({param.MachineShiftDetailIdList?.JoinAsString(",")})")
                   .Replace("@ShiftSolutionName", $@"('{param.ShiftSolutionName?.JoinAsString(",")}')");

                var result = conn.Query(sql).Select(x => (ExpandoObject)ToExpandoDynamic(x)).ToList();

                if (enmuStatisticalWays == EnumStatisticalWays.ByShift || enmuStatisticalWays == EnumStatisticalWays.ByMachineShiftDetail)
                {
                    var list = new List<ExpandoObject>();

                    foreach (var item in result)
                    {
                        var shiftName = item.FirstOrDefault(i => i.Key == "ShiftItemName").Value;
                        var shiftDay = item.FirstOrDefault(i => i.Key == "Dimensions").Value;
                        IDictionary<string, object> expando = new ExpandoObject();

                        foreach (var ite in item)
                        {

                            if (ite.Key == "Dimensions")
                            {

                                expando.Add(ite.Key, shiftDay + "-" + shiftName);

                            }
                            else if (!(ite.Key == "ShiftItemName" || ite.Key == "ShiftItemId"))
                            {
                                expando.Add(ite.Key, ite.Value?.ToString() ?? "0.00");
                            }

                        }

                        list.Add((ExpandoObject)expando);
                    }

                    return list;
                }

                return result;
            }
        }

        private static dynamic ToExpandoDynamic(object value)
        {
            var dapperRowProperties = value as IDictionary<string, object>;

            IDictionary<string, object> expando = new ExpandoObject();

            if (dapperRowProperties == null) return (ExpandoObject)expando;

            foreach (var property in dapperRowProperties) expando.Add(property.Key, property.Value);

            return (ExpandoObject)expando;
        }
    }
}