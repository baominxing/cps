using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Castle.Core.Internal;
using Wimi.BtlCore.BasicData.Capacities;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines.Repository;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.CommonEnums;
using Wimi.BtlCore.Plan;
using Wimi.BtlCore.Plan.Repository;
using Wimi.BtlCore.Plan.Repository.Dto;
using Wimi.BtlCore.ThirdpartyApis.Dto;
using Wimi.BtlCore.ThirdpartyApis.Interfaces;

namespace Wimi.BtlCore.ThirdpartyApis.Managers
{
    public class PlanComponentManager : IPlanComponentManager
    {
        private readonly IShiftDetailRepository shiftDetailRepository;
        private readonly IRepository<Capacity> capacityRepository;
        private readonly DeviceGroupManager deviceGroupManager;
        private readonly IPlanRepository processPlanRepository;
        private readonly IRepository<DeviceGroup> deviceGroupRepository;

        public PlanComponentManager(IShiftDetailRepository shiftDetailRepository,
                                           IRepository<Capacity> capacityRepository,
                                           DeviceGroupManager deviceGroupManager,
                                           IPlanRepository processPlanRepository,
                                           IRepository<DeviceGroup> deviceGroupRepository)
        {
            this.shiftDetailRepository = shiftDetailRepository;
            this.capacityRepository = capacityRepository;
            this.deviceGroupManager = deviceGroupManager;
            this.processPlanRepository = processPlanRepository;
            this.deviceGroupRepository = deviceGroupRepository;
        }

        public async Task<ApiResponseObject> ListPlanRate()
        {
            List<DeviceGroup> deviceGroups = null;
            var currentShiftInfo = this.shiftDetailRepository.GetCurrentShiftDay();

            if(currentShiftInfo.ShiftDay == new DateTime(1900, 01, 01))
            {
                deviceGroups = await deviceGroupManager.ListDeviceGroupIdsForMachineShiftEffectiveInterval();
                currentShiftInfo.ShiftDay = DateTime.Today;
            }
            else
            {
                deviceGroups = (await deviceGroupManager.ListFirstClassDeviceGroups()).ToList();
            }

            var input = new ProcessPlanInput();
            input.StartTime = currentShiftInfo.ShiftDay;
            input.EndTime = currentShiftInfo.ShiftDay.AddDays(1);
            var result = this.ListProcessPlanRate(input);

            var targetList = new List<string>();
            var legendList = new List<string>();

            var data = new List<IEnumerable<dynamic>>();
            deviceGroups.ForEach(d =>
            {

                var planList = result
                    .Where(r => r.DeviceGroupId == d.Id).Join(this.deviceGroupRepository.GetAll(), q => q.DeviceGroupId, dg => dg.Id,
                (pp,dg ) => new { DeviceGroup = dg, Plan = pp }).Select(
                        item => new List<dynamic>
                        {
                                    item.DeviceGroup.DisplayName,
                                    item.Plan.PlanName,
                                    item.Plan.ProductName,
                                    item.Plan.PlanAmount,
                                    item.Plan.CompleteAmount,
                                    item.Plan.TotalCompleteRate,
                                    item.Plan.StatisticalWay,
                                    item.Plan.StatisticalWayAmount,
                                    item.Plan.SummaryDate,
                                    item.Plan.SummaryDateAmount,
                                    item.Plan.SummaryDateCompleteRate
                        }).ToList();

                if (planList.Any())
                {
                    data.AddRange(planList);
                    targetList.Add(d.Code);
                    legendList.Add(d.DisplayName);
                }
                //else
                //{
                //    var dataItem = new List<dynamic>
                //        {
                //            d.DisplayName, "", "", "", "","", "按天", "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", ""
                //        };
                //    data.Add(dataItem);
                //}

            });

            return new ApiResponseObject(ApiItemType.Strings, ApiTargetType.One)
            {
                FixedSeries = true,
                Series = new List<IEnumerable<dynamic>>()
                    {
                        new List<dynamic>
                            {"设备组","计划名称", "产品名称", "计划产量", "完成产量", "总达成率", "目标维度", "目标量", "统计日期", "日期完成量","统计日期目标达成率" }
                    },
                Data = data,
                Target = targetList,
                Legend = legendList
            };

        }

        #region GetPlanRate
        private IEnumerable<ProductPlanYieldDto> GetPlanRate()
        {
            var currentShiftInfo = this.shiftDetailRepository.GetCurrentShiftDay();

            var shiftSolutionItemName = currentShiftInfo.ShiftSolutionItemName;

            var returnedValue = new List<ProductPlanYieldDto>();

            var weekDayRange = this.shiftDetailRepository.CorrectQueryDate(new GetMachineStateRateInputDto
            {
                StartTime = currentShiftInfo.ShiftDay.ToString("yyyy-MM-dd"),
                EndTime = currentShiftInfo.ShiftDay.ToString("yyyy-MM-dd"),
                StatisticalWay = EnumStatisticalWays.ByWeek
            });
            if (weekDayRange.IsNullOrEmpty())
            {
                return returnedValue;
            }
            var weekStartTime = Convert.ToDateTime(weekDayRange.FirstOrDefault().ShiftDay);
            var weekEndTime = Convert.ToDateTime(weekDayRange.LastOrDefault().ShiftDay);


            var monthDayRange = this.shiftDetailRepository.CorrectQueryDate(new GetMachineStateRateInputDto
            {
                StartTime = currentShiftInfo.ShiftDay.ToString("yyyy-MM-dd"),
                EndTime = currentShiftInfo.ShiftDay.ToString("yyyy-MM-dd"),
                StatisticalWay = EnumStatisticalWays.ByMonth
            });
            if (monthDayRange.IsNullOrEmpty())
            {
                return returnedValue;
            }
            var monthStartTime = Convert.ToDateTime(monthDayRange.First().ShiftDay);
            var monthEndTime = Convert.ToDateTime(monthDayRange.Last().ShiftDay);

            // 获取产品名称，计划名称，计划量，完成量，达成率部分数据
            var firstPartData = (from c in this.capacityRepository.GetAll()
                                 where c.PlanId.HasValue && c.IsLineOutput && c.Tag != "1" && c.Qualified.Value
                                 group new { c.PlanId, c.ProductName, c.PlanName, c.PlanAmount, c.Yield } by new { c.PlanId, c.ProductName, c.PlanName, c.PlanAmount } into cg
                                 select new
                                 {
                                     cg.Key.ProductName,
                                     cg.Key.PlanName,
                                     cg.Key.PlanId,
                                     PlanAmount = (int)cg.Key.PlanAmount,
                                     CompletedAmount = (int)cg.Sum(s => s.Yield)
                                 }).ToList().Select(s => new ProductPlanYieldDto
                                 {
                                     PlanId = s.PlanId,
                                     ProductName = s.ProductName,
                                     PlanName = s.PlanName,
                                     PlanAmount = s.PlanAmount,
                                     CompletedAmount = s.CompletedAmount,
                                     CompleteRate = (s.CompletedAmount / (double)s.PlanAmount).ToString("P")
                                 });

            // 获取产品计划当前班次记录
            var shiftPartData = (from c in this.capacityRepository.GetAll()
                                 where c.ShiftDetail.ShiftDay >= currentShiftInfo.ShiftDay &&
                                       c.ShiftDetail.ShiftDay <= currentShiftInfo.ShiftDay && c.PlanId != 0 &&
                                       c.ShiftDetail.MachineShiftName == shiftSolutionItemName &&
                                       c.IsLineOutput && c.Tag != "1" && c.Qualified.Value
                                 group new { c.PlanId, c.ProductName, c.PlanName, c.Yield }
                                     by new { c.PlanId, c.ProductName, c.PlanName }
                into cg
                                 select new ProductPlanYieldDto
                                 {
                                     PlanId = cg.Key.PlanId,
                                     ProductName = cg.Key.ProductName,
                                     PlanName = cg.Key.PlanName,
                                     ShiftAmount = (int)cg.Sum(s => s.Yield)
                                 }).ToList();

            // 获取产品计划当前班次日记录
            var shiftDayPartData = (from c in this.capacityRepository.GetAll()
                                    where c.ShiftDetail.ShiftDay >= currentShiftInfo.ShiftDay &&
                                          c.ShiftDetail.ShiftDay <= currentShiftInfo.ShiftDay && c.PlanId != 0 &&
                                          c.IsLineOutput && c.Tag != "1" && c.Qualified.Value
                                    group new { c.PlanId, c.ProductName, c.PlanName, c.Yield }
                                        by new { c.PlanId, c.ProductName, c.PlanName }
                into cg
                                    select new ProductPlanYieldDto
                                    {
                                        PlanId = cg.Key.PlanId,
                                        ProductName = cg.Key.ProductName,
                                        PlanName = cg.Key.PlanName,
                                        ShiftDayAmount = (int)cg.Sum(s => s.Yield)
                                    }).ToList();

            // 获取产品计划当前班次周记录
            var shiftWeekPartData = (from c in this.capacityRepository.GetAll()
                                     where c.ShiftDetail.ShiftDay >= weekStartTime && c.ShiftDetail.ShiftDay <= weekEndTime && c.PlanId != 0 &&
                                     c.IsLineOutput && c.Tag != "1" && c.Qualified.Value
                                     group new { c.PlanId, c.ProductName, c.PlanName, c.Yield }
                                         by new { c.PlanId, c.ProductName, c.PlanName }
                into cg
                                     select new ProductPlanYieldDto
                                     {
                                         PlanId = cg.Key.PlanId,
                                         ProductName = cg.Key.ProductName,
                                         PlanName = cg.Key.PlanName,
                                         ShiftWeekAmount = (int)cg.Sum(s => s.Yield)
                                     }).ToList();

            // 获取产品计划当前班次月记录
            var shiftMonthPartData = (from c in this.capacityRepository.GetAll()
                                      where c.ShiftDetail.ShiftDay >= monthStartTime && c.ShiftDetail.ShiftDay <= monthEndTime &&
                                            c.PlanId != 0 && c.IsLineOutput && c.Tag != "1" && c.Qualified.Value
                                      group new { c.PlanId, c.ProductName, c.PlanName, c.Yield }
                                          by new { c.PlanId, c.ProductName, c.PlanName }
                into cg
                                      select new ProductPlanYieldDto
                                      {
                                          PlanId = cg.Key.PlanId,
                                          ProductName = cg.Key.ProductName,
                                          PlanName = cg.Key.PlanName,
                                          ShiftMonthAmount = (int)cg.Sum(s => s.Yield)
                                      }).ToList();

           

            returnedValue = (from f in firstPartData
                             join s in shiftPartData on new { f.PlanId, f.ProductName, f.PlanName } equals new { s.PlanId, s.ProductName, s.PlanName } into leftshiftPartData
                             join d in shiftDayPartData on new { f.PlanId, f.ProductName, f.PlanName } equals new { d.PlanId, d.ProductName, d.PlanName } into leftshiftDayPartData
                             join w in shiftWeekPartData on new { f.PlanId, f.ProductName, f.PlanName } equals new { w.PlanId, w.ProductName, w.PlanName } into leftshiftWeekPartData
                             join m in shiftMonthPartData on new { f.PlanId, f.ProductName, f.PlanName } equals new { m.PlanId, m.ProductName, m.PlanName } into leftshiftMonthPartData
                             from ls in leftshiftPartData.DefaultIfEmpty()
                             from ld in leftshiftDayPartData.DefaultIfEmpty()
                             from lw in leftshiftWeekPartData.DefaultIfEmpty()
                             from lm in leftshiftMonthPartData.DefaultIfEmpty()
                             select new ProductPlanYieldDto
                             {
                                 PlanId = f.PlanId,
                                 ProductName = f.ProductName,
                                 PlanName = f.PlanName,
                                 PlanAmount = f.PlanAmount,
                                 CompletedAmount = f.CompletedAmount,
                                 CompleteRate = f.CompleteRate,
                                 ShiftAmount = ls?.ShiftAmount ?? 0,
                                 ShiftDayAmount = ld?.ShiftDayAmount ?? 0,
                                 ShiftWeekAmount = lw?.ShiftWeekAmount ?? 0,
                                 ShiftMonthAmount = lm?.ShiftMonthAmount ?? 0
                             }).ToList();

            return returnedValue;
        }
        #endregion

        private List<ProcessPlanRateDto> ListProcessPlanRate(ProcessPlanInput input)
        {
            var plans = this.processPlanRepository.ListProcessPlans(input);
            var result = new List<ProcessPlanRateDto>();

            foreach (var item in plans)
            {
                var request = new ProcessPlanInput
                {
                    StartTime = input.StartTime,
                    EndTime = input.EndTime,
                    PlanId = item.Id,
                    TargetType = item.TargetType,
                    MachineId = item.YieldCounterMachineId
                };
                if (item.YieldSummaryType == EnumYieldSummaryType.ByYieldCounter)
                {
                    var resultQuery = from p in (this.processPlanRepository.ListStatisticalWayYieldByCapacity(request))
                                      select new ProcessPlanRateDto
                                      {
                                          DeviceGroupId = item.DeviceGroupId,
                                          PlanCode = item.PlanCode,
                                          PlanName = item.PlanName,
                                          ProductName = item.ProductName,
                                          PlanAmount = item.PlanAmount,
                                          CompleteAmount = item.ProcessAmount,
                                          TotalCompleteRate = (Convert.ToDecimal(item.ProcessAmount) / item.PlanAmount).ToString("P"),
                                          StatisticalWayAmount = item.TargetType == EnumTargetDimension.ByShift ? p.TargetAmount : item.TargetAmount,
                                          SummaryDate = p.StatisticalWay,
                                          SummaryDateAmount = Convert.ToInt32(p.StatisticalWayAmount),
                                          SummaryDateCompleteRate = item.TargetType == EnumTargetDimension.ByShift ? p.TargetAmount == 0 ? 0.ToString("P") : (Convert.ToDecimal(p.StatisticalWayAmount) / p.TargetAmount).ToString("P") : (Convert.ToDecimal(p.StatisticalWayAmount) / item.TargetAmount).ToString("P"),
                                          StatisticalWay = GetStatisticalName(item.TargetType)
                                      };
                    result.AddRange(resultQuery.ToList());

                }
                else
                {
                    var resultQuery = from p in (this.processPlanRepository.ListStatisticalWayYieldByTrace(request))
                                      select new ProcessPlanRateDto
                                      {
                                          DeviceGroupId = item.DeviceGroupId,
                                          PlanCode = item.PlanCode,
                                          PlanName = item.PlanName,
                                          ProductName = item.ProductName,
                                          PlanAmount = item.PlanAmount,
                                          CompleteAmount = item.ProcessAmount,
                                          TotalCompleteRate = (Convert.ToDecimal(item.ProcessAmount) / item.PlanAmount).ToString("P"),
                                          StatisticalWayAmount = item.TargetType == EnumTargetDimension.ByShift ? p.TargetAmount : item.TargetAmount,
                                          SummaryDate = p.StatisticalWay,
                                          SummaryDateAmount = Convert.ToInt32(p.StatisticalWayAmount),
                                          SummaryDateCompleteRate = item.TargetType == EnumTargetDimension.ByShift ? p.TargetAmount == 0 ? 0.ToString("P") : (Convert.ToDecimal(p.StatisticalWayAmount) / p.TargetAmount).ToString("P") : (Convert.ToDecimal(p.StatisticalWayAmount) / item.TargetAmount).ToString("P"),
                                          StatisticalWay = GetStatisticalName(item.TargetType)
                                      };
                    result.AddRange(resultQuery.ToList());

                }
            }
            return result;
        }

        private string GetStatisticalName(EnumTargetDimension input)
        {
            var result = string.Empty;
            switch (input)
            {
                case EnumTargetDimension.ByDay:
                    result = "按天";
                    break;
                case EnumTargetDimension.ByWeek:
                    result = "按周";
                    break;
                case EnumTargetDimension.ByMonth:
                    result = "按月";
                    break;
                case EnumTargetDimension.ByYear:
                    result = "按年";
                    break;
                case EnumTargetDimension.ByShift:
                    result = "按班次";
                    break;
            }
            return result;
        }

    }
}