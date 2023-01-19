using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Archives;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Machines.Repository;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.BasicData.States;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Plan;
using Wimi.BtlCore.Plan.Repository;
using Wimi.BtlCore.Plan.Repository.Dto;
using Wimi.BtlCore.RDLCReport.Dto;
using Wimi.BtlCore.StatisticAnalysis.Yield;

namespace Wimi.BtlCore.RDLCReport
{
    public class RDLCReportService : BtlCoreAppServiceBase, IRDLCReportService
    {
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<User, long> userRepository;
        private readonly YieldAppService yieldAppService;
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly IRepository<ProcessPlan> processPlanRepository;
        private readonly IRepository<ShiftSolution> ssRepository;
        private readonly IRepository<DeviceGroup> deviceGroupRepository;
        private readonly IRepository<State, long> stateRepository;
        private readonly IRepository<MachineDeviceGroup> machineDeviceRepository;
        private readonly IRepository<StateInfo> stateInfoRepository;
        private readonly IRepository<ShiftSolutionItem> shiftSolusionItemRepository;
        private readonly IRepository<ShiftSolution> shiftSolutionRepository;
        private readonly IPlanRepository planRopertRepository;
        private readonly IRepository<PlanTarget> planTargetRepository;
        private readonly IRepository<ShiftCalendar, long> scRepository;
        private readonly IRepository<ArchiveEntry> archiveEntryRepository;
        private readonly IStateRepository statesRepository;
        private readonly ICommonRepository commonRepository;

        public RDLCReportService(IRepository<Machine> machineRepository,
            IRepository<User, long> userRepository,
            YieldAppService yieldAppService,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<ProcessPlan> processPlanRepository,
            IRepository<ShiftSolution> ssRepository,
            IRepository<DeviceGroup> deviceGroupRepository,
            IRepository<State, long> stateRepository,
            IRepository<MachineDeviceGroup> machineDeviceRepository,
            IRepository<StateInfo> stateInfoRepository,
            IRepository<ShiftSolutionItem> shiftSolusionItemRepository,
            IRepository<ShiftSolution> shiftSolutionRepository,
            IPlanRepository planRopertRepository,
            IRepository<PlanTarget> planTargetRepository,
            IRepository<ShiftCalendar, long> scRepository,
            IRepository<ArchiveEntry> archiveEntryRepository,
            IStateRepository statesRepository,
            ICommonRepository commonRepository)
        {
            this.machineRepository = machineRepository;
            this.userRepository = userRepository;
            this.yieldAppService = yieldAppService;
            this.unitOfWorkManager = unitOfWorkManager;
            this.processPlanRepository = processPlanRepository;
            this.ssRepository = ssRepository;
            this.deviceGroupRepository = deviceGroupRepository;
            this.stateRepository = stateRepository;
            this.machineDeviceRepository = machineDeviceRepository;
            this.stateInfoRepository = stateInfoRepository;
            this.shiftSolusionItemRepository = shiftSolusionItemRepository;
            this.shiftSolutionRepository = shiftSolutionRepository;
            this.planRopertRepository = planRopertRepository;
            this.planTargetRepository = planTargetRepository;
            this.scRepository = scRepository;
            this.archiveEntryRepository = archiveEntryRepository;
            this.statesRepository = statesRepository;
            this.commonRepository = commonRepository;
        }

        /// <summary>
        /// 状态用时占比
        /// </summary>
        /// <param name="startTime">班次日开始时间</param>
        /// <param name="endTime">班次日结束时间</param>
        /// <returns></returns>
        [HttpPost]
        public IList<StateConsumeTimeDto> GetStateConsumeTimeReportData(ReportInputDto input)
        {
            var unionTables = commonRepository.GetUnionTables(input.StartTime, input.EndTime, "States");
            var stateQuery = statesRepository.QueryStatesBetweenStartTimeAndEndTime(input.StartTime, input.EndTime, unionTables);

            if (!stateQuery.Any())
            {
                return new List<StateConsumeTimeDto>();
            }

            var query = stateQuery.GroupBy(t => new
            {
                t.MachineName,
                t.MachineGroupName,
                t.MachineId,
                t.ShiftDetail_ShiftDay,
                t.ShiftDetail_SolutionName,
                t.ShiftDetail_MachineShiftName,
                t.StartTime
            }, (key, g) => new
            {
                key.MachineGroupName,
                key.MachineName,
                Date = key.ShiftDetail_ShiftDay,
                ShiftSolutionName = key.ShiftDetail_SolutionName,
                Shift = key.ShiftDetail_MachineShiftName,
                StartTime = key.StartTime,
                RunDuration = g.Where(s => s.Code == "Run").Sum(s => (double)s.Duration),
                DebugDuration = g.Where(s => s.Code == "Debug").Sum(s => (double)s.Duration),
                StopDuration = g.Where(s => s.Code == "Stop").Sum(s => (double)s.Duration),
                FreeDuration = g.Where(s => s.Code == "Free").Sum(s => (double)s.Duration),
                OfflineDuration = g.Where(s => s.Code == "Offline").Sum(s => (double)s.Duration),
                TotalDuration = g.Sum(s => (double)s.Duration),
            }).ToList();

            var result = query.Select(t => new StateConsumeTimeDto
            {
                MachineGroupName = t.MachineGroupName,
                MachineName = t.MachineName,
                Date = t.Date.ToString("yyyy-MM-dd"),
                ShiftSolutionName = t.ShiftSolutionName,
                Shift = t.Shift,
                StartDate = t.StartTime,
                RunTime = Math.Round(t.RunDuration / 3600, 2).ToString(),
                RunPercent = t.RunDuration == 0 ? "0" : Math.Round(t.RunDuration / t.TotalDuration * 1.0, 4).ToString("P2"),
                StopTime = Math.Round(t.StopDuration / 3600, 2).ToString(),
                StopPercent = t.StopDuration == 0 ? "0" : Math.Round(t.StopDuration / t.TotalDuration * 1.0, 4).ToString("P2"),
                OfflineTime = Math.Round(t.OfflineDuration / 3600, 2).ToString(),
                OfflinePercent = t.OfflineDuration == 0 ? "0" : Math.Round(t.OfflineDuration / t.TotalDuration * 1.0, 4).ToString("P2"),
                AvaliableTime = Math.Round(t.FreeDuration / 3600, 2).ToString(),
                AvaliablePercent = t.FreeDuration == 0 ? "0" : Math.Round(t.FreeDuration / t.TotalDuration * 1.0, 4).ToString("P2"),
                DebugTime = Math.Round(t.DebugDuration / 3600, 2).ToString(),
                DebugPercent = t.DebugDuration == 0 ? "0" : Math.Round(t.DebugDuration / t.TotalDuration * 1.0, 4).ToString("P2"),

            }).OrderBy(x => x.MachineGroupName).ThenBy(x => x.MachineName).ThenBy(x => x.StartDate).ToList();

            return result;
        }

        /// <summary>
        /// 设备稼动率
        /// </summary>
        /// <param name="startTime">班次日开始时间</param>
        /// <param name="endTime">班次日结束时间</param>
        /// <returns></returns>
        [HttpPost]
        public IList<MachineUtilizationRateDto> GetMachineUtilizationRateData(ReportInputDto input)
        {
            IList<MachineUtilizationRateDto> list = new List<MachineUtilizationRateDto>();
            var unionTables = commonRepository.GetUnionTables(input.StartTime, input.EndTime, "States");

            var stateQuery = statesRepository.QueryStatesBetweenStartTimeAndEndTime(input.StartTime, input.EndTime, unionTables);

            if (!stateQuery.Any())
            {
                return list;
            }
            var planedCodes = this.stateInfoRepository.GetAll().Where(t => t.IsStatic && t.IsPlaned).Select(t => t.Code).ToList();

            var query = stateQuery.GroupBy(t => new
            {
                t.MachineName,
                t.MachineGroupName,
                t.MachineId,
                t.ShiftDetail_ShiftDay
            }, (key, g) => new
            {
                key.MachineGroupName,
                key.MachineName,
                Date = key.ShiftDetail_ShiftDay,
                PlanedDuration = g.Where(s => planedCodes.Contains(s.Code)).Sum(s => (double)s.Duration),
                TotalDuration = g.Sum(s => (double)s.Duration),
            }).ToList();


            return query.Select(t => new MachineUtilizationRateDto()
            {
                MachineGroup = t.MachineGroupName,
                Machine = t.MachineName,
                Date = t.Date.ToString("yyyy-MM-dd"),
                UtilizationRate = t.PlanedDuration == 0 ? "0" : Math.Round(t.PlanedDuration / t.TotalDuration * 1.0, 4).ToString("P2")
            }).OrderBy(t => t.MachineGroup).ThenBy(t => t.Machine).ThenBy(t => t.Date).ToList();
        }

        /// <summary>
        /// 人员产量统计
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IList<PersonYieldDto>> GetPersonYieldReportData(ReportInputDto input)
        {
            IList<PersonYieldDto> list = new List<PersonYieldDto>();
            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();
            var unionTables = commonRepository.GetUnionTables(input.StartTime, input.EndTime, "Capacities");
            var capacitiesQuery = statesRepository.QueryCapacitiesBetweenStartTimeAndEndTime(input.StartTime, input.EndTime, unionTables);
            //stopwatch.Stop();
            //System.Diagnostics.Debug.Write("一方法耗时（毫秒）:" + stopwatch.ElapsedMilliseconds+"/n");
            if (!capacitiesQuery.Any())
            {
                return list;
            }
            //System.Diagnostics.Stopwatch stopwatch2 = new System.Diagnostics.Stopwatch();
            //stopwatch2.Start();
            var query = (from c in capacitiesQuery
                         join u in userRepository.GetAll() on new { UserId = c.UserId != null ? (long)c.UserId : 0 } equals new { UserId = u.Id }
                         select new
                         {
                             c.UserId,
                             UserName = u.Name,
                             ShiftDay = c.ShiftDetail_ShiftDay,
                             SolutionId = c.ShiftSolutionId,
                             SolutionName = c.ShiftDetail_SolutionName,
                             ShiftName = c.ShiftDetail_MachineShiftName,
                             Yield = (int)c.Yield,
                             ShiftDuration = c.Duration,
                             ShiftStartTime = c.StartTime,
                             ShiftEndTime = c.EndTime,
                             c.IsNextDay
                         }).ToLookup(c => new { c.UserId, c.ShiftDay, c.SolutionId, c.SolutionName, c.ShiftName }).ToList();

            //stopwatch2.Stop();
            ////Logger.Info("2方法耗时（毫秒）:" + stopwatch2.ElapsedMilliseconds);
            //System.Diagnostics.Debug.Write("二方法耗时（毫秒）:" + stopwatch2.ElapsedMilliseconds + "/n");

            if (input.ShiftSolutionId != 0)
            {
                query = query.Where(s => s.Key.SolutionId == input.ShiftSolutionId).ToList();
            }

            foreach (var user in query)
            {
                var personYieldDto = new PersonYieldDto
                {
                    UserId = Convert.ToInt32(user.Key.UserId),
                    SolutionName = user.Key.SolutionName,
                    Date = ((DateTime)user.Key.ShiftDay).ToString("yyyy-MM-dd"),
                    ShiftName = user.Key.ShiftName,
                    UserName = user.FirstOrDefault()?.UserName,
                    Yield = user.Sum(c => c.Yield)
                };
                if (personYieldDto.Yield != 0)
                {
                    var item = user.FirstOrDefault();
                    var start = item.ShiftStartTime;
                    var startInterval = start.Hour * 3600 + start.Minute * 60 + start.Second;
                    var end = item.ShiftEndTime;
                    var endInterval = end.Hour * 3600 + end.Minute * 60 + end.Second;

                    var realStart = user.Key.ShiftDay.AddSeconds(startInterval);
                    if (item.IsNextDay)
                    {
                        endInterval += 24 * 3600;
                    }
                    var current = DateTime.Now;
                    var realEnd = user.Key.ShiftDay.AddSeconds(endInterval);
                    var duration = item.ShiftDuration * 1d;
                    if (current < realEnd)
                    {
                        duration = Convert.ToDouble((current - realStart).TotalSeconds);
                    }
                    var seconds = Math.Round(duration / (double)personYieldDto.Yield);
                    var hour = Math.Floor(seconds / 3600);
                    var mimute = Math.Floor(seconds % 3600 / 60);
                    var second = seconds % 60;
                    personYieldDto.AverageRhythm = (hour == 0 ? "" : (hour + ":")) + mimute.ToString() + ":" + second;
                }
                list.Add(personYieldDto);
            }

            var result = list.OrderBy(t => t.UserName).ThenBy(t => t.Date).ThenBy(t => t.ShiftDate).ToList();
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 人员绩效统计
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpPost]
        public IList<PersonPerfomanceDto> GetPersonPerformanceReportData(ReportInputDto input)
        {
            var list = new List<PersonPerfomanceDto>();

            var unionTables = commonRepository.GetUnionTables(input.StartTime, input.EndTime, "States");

            var stateQuery = statesRepository.QueryStatesBetweenStartTimeAndEndTime(input.StartTime, input.EndTime, unionTables, true);

            if (!stateQuery.Any())
            {
                return list;
            }
            var query = (from s in stateQuery.ToList()
                         join m in machineRepository.GetAllList() on new { MachineId = (int)s.MachineId } equals new { MachineId = m.Id }
                         join u in userRepository.GetAllList() on new { UserId = (long)s.UserId } equals new { UserId = u.Id }
                         where s.UserId != null
                         select new PersonPerfomanceDto
                         {
                             StartTime = s.StartTime,
                             EndTime = s.EndTime,
                             DateTime = s.ShiftDetail_ShiftDay,
                             UserId = u.Id,
                             UserName = u.Name,
                             Duration = s.Duration,
                             MachineId = m.Id,
                             MachineName = m.Name,
                             State = s.Name,
                             StateCode = s.Code
                         }).GroupBy(g => new { g.UserId, g.MachineId, g.DateTime }).ToList();
            foreach (var item in query)
            {
                //获取总的持续时间
                var totalCount = 0.0m;
                foreach (var ite in item)
                {
                    if (ite.Duration != 0)
                    {
                        totalCount += ite.Duration;
                    }
                    else
                    {
                        if (ite.StartTime != null && ite.EndTime == null)
                        {
                            totalCount += Convert.ToDecimal((DateTime.Now - ite.StartTime.Value).TotalSeconds);
                        }
                    }
                }
                var stateGroup = item.GroupBy(g => g.StateCode);
                foreach (var state in stateGroup)
                {
                    var data = state.FirstOrDefault();

                    data.Date = data.DateTime?.ToString("yyyy-MM-dd");
                    var itemDuration = 0.0m;
                    foreach (var s in state)
                    {
                        if (s.Duration != 0)
                        {
                            itemDuration += s.Duration;
                        }
                        else
                        {
                            if (s.StartTime != null && s.EndTime == null)
                            {
                                itemDuration += Convert.ToDecimal((DateTime.Now - s.StartTime.Value).TotalSeconds);
                            }
                        }
                    }
                    if (totalCount == 0)
                    {
                        data.Persent = "0";
                    }
                    else
                    {
                        data.Persent = (Math.Round((itemDuration / totalCount) * 10000) / 100).ToString() + "%";
                    }
                    data.Duration = Math.Round(itemDuration / 3600 * 100) / 100;
                    list.Add(data);
                }
            }
            return list;
        }

        /// <summary>
        /// 计划达成报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IList<ProcessPlanResultDto> ListProcessPlanYield(ProductPlanYieldInputDto input)
        {
            var endTime = input.EndTime.AddDays(1);

            var query = this.planRopertRepository.ListProcessPlans(new ProcessPlanInput { StartTime = input.StartTime, EndTime = endTime });

            var result = new List<ProcessPlanResultDto>();

            foreach (var item in query)
            {
                var request = new ProcessPlanInput
                {
                    StartTime = input.StartTime,
                    EndTime = endTime,
                    PlanId = item.Id,
                    TargetType = item.TargetType,
                    MachineId = item.YieldCounterMachineId,
                    UnionTables = commonRepository.GetUnionTables(input.StartTime, endTime, "Capacities")
                };
                if (item.YieldSummaryType == EnumYieldSummaryType.ByYieldCounter)
                {
                    var resultQuery = from p in (this.planRopertRepository.ListStatisticalWayYieldByCapacity(request))
                                      select new ProcessPlanResultDto
                                      {
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

                    var list = this.Filling(resultQuery.ToList(), request, item);
                    result.AddRange(list);
                }
                else
                {
                    request.UnionTables = commonRepository.GetUnionTables(input.StartTime, endTime, "TraceCatalogs");

                    var resultQuery = from p in (this.planRopertRepository.ListStatisticalWayYieldByTrace(request))
                                      select new ProcessPlanResultDto
                                      {
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

                    var list = this.Filling(resultQuery.ToList(), request, item);
                    result.AddRange(list);
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
                    result = this.L("ByDay");
                    break;

                case EnumTargetDimension.ByWeek:
                    result = this.L("ByWeek");
                    break;

                case EnumTargetDimension.ByMonth:
                    result = this.L("ByMonth");
                    break;

                case EnumTargetDimension.ByYear:
                    result = this.L("ByYear");
                    break;

                case EnumTargetDimension.ByShift:
                    result = this.L("ByShift");
                    break;
            }
            return result;
        }

        private List<ProcessPlanResultDto> Filling(List<ProcessPlanResultDto> input, ProcessPlanInput plan, PlanResponse response)
        {
            var result = new List<ProcessPlanResultDto>();
            plan.StartTime = response.RealStartTime.Date >= plan.StartTime ? response.RealStartTime.Date : plan.StartTime;
            plan.EndTime = plan.EndTime.AddDays(-1);
            var DateRange = this.planRopertRepository.ListSummaryDate(plan).Select(d => new { d.Value, d.ShiftId }).Distinct();

            foreach (var dateRange in DateRange)
            {
                var targetAmount = 0;
                if (response.TargetType == EnumTargetDimension.ByShift)
                {
                    var planRecord = this.processPlanRepository.GetAll().Where(p => p.Id == response.Id).Include(p => p.ShiftTarget).First();
                    var targetRecord = planRecord.ShiftTarget.FirstOrDefault(t => t.ShiftId == dateRange.ShiftId);
                    if (targetRecord != null)
                    {
                        targetAmount = targetRecord.ShiftTargetAmount;
                    }
                }

                var entity = input.FirstOrDefault(i => dateRange.Value.Equals(i.SummaryDate));
                var dto = ObjectMapper.Map<ProcessPlanResultCopyDto>(entity);
                if (entity != null)
                {
                    result.Add(dto);
                }
                else
                {
                    result.Add(new ProcessPlanResultDto
                    {
                        PlanCode = response.PlanCode,
                        PlanName = response.PlanName,
                        ProductName = response.ProductName,
                        PlanAmount = response.PlanAmount,
                        CompleteAmount = response.ProcessAmount,
                        TotalCompleteRate = (Convert.ToDecimal(response.ProcessAmount) / response.PlanAmount).ToString("P"),
                        StatisticalWayAmount = response.TargetType == EnumTargetDimension.ByShift ? targetAmount : response.TargetAmount,
                        SummaryDate = dateRange.Value,
                        SummaryDateAmount = 0,
                        SummaryDateCompleteRate = 0.ToString("P"),
                        StatisticalWay = GetStatisticalName(response.TargetType)
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// 获取班次方案列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<dynamic>> GetShiftSolutionList()
        {
            return
                await
                (from u in this.ssRepository.GetAll()
                 select new { ShiftSolutionId = u.Id, ShiftSolutionName = u.Name }).ToListAsync();
        }
    }
}