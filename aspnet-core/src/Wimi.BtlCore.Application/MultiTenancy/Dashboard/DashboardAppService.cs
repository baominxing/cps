using Abp;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Threading;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Archives;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BasicData.Capacities.Manager;
using Wimi.BtlCore.BasicData.Capacities.Mongo;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Machines.Repository;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Dashboard;
using Wimi.BtlCore.Dashboard.Dtos;
using Wimi.BtlCore.EfficiencyTrendas.Dtos;
using Wimi.BtlCore.Machines.Mongo;
using Wimi.BtlCore.MultiTenancy.Dashboard.Dto;
using Wimi.BtlCore.ShiftDayTimeRange;
using Wimi.BtlCore.States;
using Wimi.BtlCore.States.Mongo;
using Wimi.BtlCore.StatisticAnalysis.EfficiencyTrends;
using Wimi.BtlCore.StatisticAnalysis.States.Dto;

namespace Wimi.BtlCore.MultiTenancy.Dashboard
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_Dashboard)]
    public class DashboardAppService : BtlCoreAppServiceBase, IDashboardAppService
    {
        private readonly ICommonLookupAppService commonLookupAppService;
        private readonly IEfficiencyTrendsAppService efficiencyTrendsAppService;
        private readonly IRepository<MachinesShiftDetail> machinesShiftDetailsRepository;
        private readonly MongoMachineManager mongoMachineManager;
        private readonly MongoStateManager mongoStateManager;
        private readonly MongoCapacityManager mongoCapacityManager;
        private readonly IActivationRepository activationRepository;
        private readonly CapacityManager capacityManager;
        private readonly StateManager stateManager;
        private readonly ISettingManager settingManager;
        private readonly IRepository<ArchiveEntry> archiveEntryRepository;
        private readonly IShiftDayTimeRangeRepository shiftDayTimeRangeRepository;
        private readonly IDashboardRepository dashboardRepository;

        public DashboardAppService(
            IEfficiencyTrendsAppService efficiencyTrendsAppService,
            ICommonLookupAppService commonLookupAppService,
            IRepository<MachinesShiftDetail> machinesShiftDetailsRepository,
            MongoMachineManager mongoMachineManager,
            MongoStateManager mongoStateManager,
            MongoCapacityManager mongoCapacityManager,
            IActivationRepository activationRepository,
            CapacityManager capacityManager,
            IShiftDayTimeRangeRepository shiftDayTimeRangeRepository,
            StateManager stateManager,
            ISettingManager settingManager,
            IRepository<ArchiveEntry> archiveEntryRepository,
            IDashboardRepository dashboardRepository)
        {
            this.efficiencyTrendsAppService = efficiencyTrendsAppService;
            this.commonLookupAppService = commonLookupAppService;
            this.machinesShiftDetailsRepository = machinesShiftDetailsRepository;
            this.mongoMachineManager = mongoMachineManager;
            this.mongoStateManager = mongoStateManager;
            this.mongoCapacityManager = mongoCapacityManager;
            this.activationRepository = activationRepository;
            this.capacityManager = capacityManager;
            this.stateManager = stateManager;
            this.settingManager = settingManager;
            this.archiveEntryRepository = archiveEntryRepository;
            this.shiftDayTimeRangeRepository = shiftDayTimeRangeRepository;
            this.dashboardRepository = dashboardRepository;
        }

        [HttpPost]
        public async Task<GetCurrentMachineShiftInfoDto> GetCurrentMachineShiftInfo()
        {
            var query = await this.GetCurrentMachineShifDetailList();
            return query.FirstOrDefault();
        }

        [HttpPost]
        public async Task<StatesStatisticForChartDto> GetDashboardStatisticForChartInGivenDays(GetStatesStatisticInGivenDaysInputDto input)
        {
            var resultDto = new StatesStatisticForChartDto();
            var statisticList = await this.GetDashboardStatisticInGivenDays(input);
            foreach (var item in statisticList)
            {
                resultDto.SummaryDate.Add(item.SummaryDate);
                resultDto.StopRate.Add(item.StopRate ?? 0);
                resultDto.RunRate.Add(item.RunRate ?? 0);
                resultDto.OfflineRate.Add(item.OfflineRate ?? 0);
                resultDto.FreeRate.Add(item.FreeRate ?? 0);
                resultDto.DebugRate.Add(item.DebugRate ?? 0);
                resultDto.Yield.Add(item.Yield ?? 0);
            }

            return resultDto;
        }

        [DisableAuditing]
        [HttpPost]
        public async Task<StatesStatisticForChartDto> GetDashboardStatisticForChartInGivenDaysByGroupId(GetStatesStatisticInGivenDaysInputDto input)
        {
            var resultDto = new StatesStatisticForChartDto();
            var statisticList = await this.GetDashboardStatisticInGivenDaysByGroupId(input);
            foreach (var item in statisticList)
            {
                resultDto.StopRate.Add(item.StopRate ?? 0);
                resultDto.RunRate.Add(item.RunRate ?? 0);
                resultDto.OfflineRate.Add(item.OfflineRate ?? 0);
                resultDto.FreeRate.Add(item.FreeRate ?? 0);
                resultDto.DebugRate.Add(item.DebugRate ?? 0);
                resultDto.SummaryDate.Add(item.SummaryDate);
            }

            return resultDto;
        }

        /// <summary>
        /// 首页已弃用方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<MachineStatisticDataDto>> GetDashboardStatisticInGivenDays(GetStatesStatisticInGivenDaysInputDto input)
        {
            return await dashboardRepository.GetDashboardStatisticInGivenDays(input);
        }

        [HttpPost]
        public async Task<IEnumerable<MachineStatisticDataDto>> GetDashboardStatisticInGivenDaysByGroupId(GetStatesStatisticInGivenDaysInputDto input)
        {
            return await dashboardRepository.GetDashboardStatisticInGivenDaysByGroupId(input);
        }

        [HttpPost]
        public async Task<GetMachineActivationDto> GetMachineActivationForDashboard()
        {
            var returnValue = new GetMachineActivationDto();

            // 获取当前登录账号权限能看到的设备列表
            var machineListWithPermissions = await this.GetDistinctedCachedMachineIdList();

            var machineIdList = machineListWithPermissions.Select(s => s.Id).ToList();
            try
            {
                // 获取当前设备班次列表
                var currentMachineShiftDetailList = (await this.GetCurrentMachineShifDetailList()).Select(s => s.MachinesShiftDetailId).ToList();

                // 获取前一个设备班次列表
                var previousMachineShiftDetailList = (await this.dashboardRepository.GetPreviousMachineShiftDetailList(currentMachineShiftDetailList)).ToList();

                if (!currentMachineShiftDetailList.Any() || !previousMachineShiftDetailList.Any())
                {
                    return returnValue;
                }

                var currentShiftCalendar = this.shiftDayTimeRangeRepository.ListMachineShiftDayTimeRange(currentMachineShiftDetailList.First());
                var previousShiftCalendar = this.shiftDayTimeRangeRepository.ListMachineShiftDayTimeRange(previousMachineShiftDetailList.First());

                if (currentShiftCalendar == null || previousShiftCalendar == null)
                {
                    Logger.Debug($"稼动率：当前班次==null[{currentShiftCalendar == null}]  前一班次 ==null[{previousShiftCalendar == null}]");
                    return returnValue;
                }

                // 获取当前班车设备稼动率
                returnValue.CurrentShiftMachineActivations = AsyncHelper.RunSync(async () => await
                        this.GetMachineActivation(
                            new EfficiencyTrendsInputDto()
                            {
                                ShiftName = currentShiftCalendar.ShiftItemName,
                                StartTime = Convert.ToDateTime(currentShiftCalendar.StartTime),
                                EndTime = Convert.ToDateTime(currentShiftCalendar.EndTime),
                                MachineId = machineIdList,
                                MachineShiftDetailId = currentMachineShiftDetailList
                            }));

                this.Logger.Debug(
                   $"Current :[{currentShiftCalendar.ShiftItemName}],[{Convert.ToDateTime(currentShiftCalendar.StartTime)}],[{Convert.ToDateTime(currentShiftCalendar.EndTime)}],[{machineIdList.JoinAsString(",")}],[{currentMachineShiftDetailList.JoinAsString(",")}]");

                // 获取前一个班次设备稼动率
                returnValue.PreviousShiftMachineActivations = AsyncHelper.RunSync(async () => await
                      this.GetMachineActivation(
                            new EfficiencyTrendsInputDto()
                            {
                                ShiftName = previousShiftCalendar.ShiftItemName,
                                StartTime = Convert.ToDateTime(previousShiftCalendar.StartTime),
                                EndTime = Convert.ToDateTime(previousShiftCalendar.EndTime),
                                MachineId = machineIdList,
                                MachineShiftDetailId = previousMachineShiftDetailList
                            }));

                this.Logger.Debug(
                     $"previous :[{previousShiftCalendar.ShiftItemName}],[{Convert.ToDateTime(previousShiftCalendar.StartTime)}],[{Convert.ToDateTime(previousShiftCalendar.EndTime)}],[{machineIdList.JoinAsString(",")}],[{previousMachineShiftDetailList.JoinAsString(",")}]");
                //补0

                foreach (var item in machineListWithPermissions)
                {
                    if (returnValue.CurrentShiftMachineActivations.All(s => s.MachineId != item.Id))
                    {
                        returnValue.CurrentShiftMachineActivations.Add(new MachineActivationDto()
                        {
                            SortSeq = item.SortSeq,
                            MachineId = item.Id,
                            MachineName = item.Name,
                            Activation = 0
                        });
                    }

                    if (returnValue.PreviousShiftMachineActivations.All(s => s.MachineId != item.Id))
                    {
                        returnValue.PreviousShiftMachineActivations.Add(new MachineActivationDto()
                        {
                            SortSeq = item.SortSeq,
                            MachineId = item.Id,
                            MachineName = item.Name,
                            Activation = 0
                        });
                    }
                }

                var previousShiftMachineActivations = returnValue.PreviousShiftMachineActivations.AsQueryable();
                returnValue.PreviousShiftMachineActivations = (from p in previousShiftMachineActivations
                                                               orderby p.SortSeq ascending, p.MachineId ascending
                                                               select p).ToList();

                var currentShiftMachineActivations = returnValue.CurrentShiftMachineActivations.AsQueryable();
                returnValue.CurrentShiftMachineActivations = (from p in currentShiftMachineActivations
                                                              orderby p.SortSeq ascending, p.MachineId ascending
                                                              select p).ToList();
            }
            catch (Exception e)
            {
                this.Logger.Fatal("首页-设备稼动率", e);
            }
            return returnValue;
        }

        [DisableAuditing]
        [HttpPost]
        public async Task<MachineStatisticDataDto> GetMachineRealTimePadData(GetMachineRealTimePadDataDto input)
        {
            var resultDto = new MachineStatisticDataDto();

            resultDto = await dashboardRepository.GetMachineStatisticData(input.MachineId);

            // 从mongo中获取状态和产量
            var capacityResult = capacityManager.GetLastCapacityRecord(input.MachineCode, 1).FirstOrDefault();
            var stateResult = stateManager.GetLastStateRecord(input.MachineCode, 1).FirstOrDefault();

            // 获取当天产出=当前最新一笔产量Count-当天第一笔Count
            var machineCount = 0m; // 设备当天产量
            var firstCapacity = capacityManager.GetFirstCapacityRecordByDay(input.MachineCode);

            if (firstCapacity != null)
            {
                var startCount = firstCapacity.AccumulateCount;
                machineCount = stateResult == null ? 0 : capacityResult.AccumulateCount - (int)startCount;
            }

            resultDto.Yield = machineCount;

            if (stateResult != null && (!string.IsNullOrEmpty(stateResult.Code)
                    && Enum.IsDefined(typeof(EnumMachineState), stateResult.Code)))
            {
                resultDto.MachineStatus = (EnumMachineState)Enum.Parse(typeof(EnumMachineState), stateResult.Code);
            }

            return resultDto;
        }

        [HttpPost]
        public async Task<GetMachineUsedTimeRateDto> GetMachineUsedTimeRateForDashboard()
        {
            var returnValue = new GetMachineUsedTimeRateDto();

            // 获取当前登录账号权限能看到的设备列表
            var machineListWithPermissions = await this.GetDistinctedCachedMachineIdList();

            var machineIdList = machineListWithPermissions.Select(s => s.Id).ToList();

            // 获取当前设备班次
            var currentMachineShiftId = (await this.GetCurrentMachineShifDetailList()).Where(s => s.MachinesShiftDetailId != 0).Select(s => s.MachinesShiftDetailId);

            if (!currentMachineShiftId.Any())
            {
                Logger.Debug($"Dashboard用时分析查询取消，当前无班次数据");
                return returnValue;
            }

            this.Logger.Debug($"首页-当天班次用时比例 参数: {currentMachineShiftId.JoinAsString(",")} - {machineIdList.JoinAsString(".")}");

            try
            {
                returnValue.CurrentShiftMachineUsedTimeRates = await dashboardRepository.QueryMachineUsedTimeRate(currentMachineShiftId);

                var flatMachineDtoList = await this.GetMachineDtoList();

                var machineIdHasPermission = flatMachineDtoList.Select(m => m.Id).ToList();

                returnValue.CurrentShiftMachineUsedTimeRates = returnValue.CurrentShiftMachineUsedTimeRates.Where(q => machineIdHasPermission.Contains(q.MachineId)).ToList();
            }
            catch (Exception e)
            {
                this.Logger.Fatal("首页-当天班次用时比例", e);
            }

            return returnValue;
        }

        [HttpPost]
        public GetMemberActivityOutputDto GetMemberActivity()
            =>
                new GetMemberActivityOutputDto
                {
                    TotalMembers =
                            Enumerable.Range(0, 13)
                            .Select(i => RandomHelper.GetRandom(15, 40))
                            .ToList(),
                    NewMembers =
                            Enumerable.Range(0, 13)
                            .Select(i => RandomHelper.GetRandom(3, 15))
                            .ToList()
                };

        private async Task<List<GetCurrentMachineShiftInfoDto>> GetCurrentMachineShifDetailList()
        {
            var machineListWithPermissions = await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();
            var machineIdList = machineListWithPermissions.Machines.Select(s => s.Id).ToList();
            var mongodata = mongoMachineManager.OriginalMongoMachineList(machineIdList);

            if (mongodata == null)
            {
                return new List<GetCurrentMachineShiftInfoDto>();
            }

            return mongodata.Select(s => new GetCurrentMachineShiftInfoDto { MachinesShiftDetailId = s.MachinesShiftDetailId }).ToList();
        }
 
        private async Task<List<FlatMachineDto>> GetDistinctedCachedMachineIdList()
        {
            var flatMachineDtoList = await this.GetMachineDtoList();

            var result = (from e in flatMachineDtoList
                          group e by new { e.Id, e.Code, e.Name, e.SortSeq }
                          into gd
                          select new FlatMachineDto { Id = gd.Key.Id, Code = gd.Key.Code, Name = gd.Key.Name, SortSeq = gd.Key.SortSeq })
                .ToList();
            return result;
        }

        private List<string> GetUnionTables(EfficiencyTrendsInputDto input)
        {
            var archiveTables = this.archiveEntryRepository.GetAll()
                .Where(s => s.TargetTable == "States").ToList()
                .Where(s => input.StartTime <= Convert.ToDateTime(s.ArchiveValue).Date && Convert.ToDateTime(s.ArchiveValue).Date <= input.EndTime)
                .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToList();

            return archiveTables;
        }

        public async Task<List<MachineActivationDto>> GetMachineActivation(EfficiencyTrendsInputDto input)
        {
            var machineListWithPermissions = await this.GetDistinctedCachedMachineIdList();
            input.UnionTables = this.GetUnionTables(input);
            input.QueryType = "0";
            input.StatisticalWays = "ByMachineShiftDetail";
            var activation = await this.activationRepository.GetMachineActivationOriginalData(input);

            if (activation == null)
            {
                throw new ArgumentNullException(nameof(activation));
            }

            var activationDictionary = new Dictionary<string, object>();

            const double TOLERANCE = 0;
            foreach (var item in activation)
            {
                foreach (var subItem in item)
                {
                    if (subItem.Key == "Dimensions")
                    {
                        if (!activationDictionary.Keys.Contains(subItem.Key))
                        {
                            activationDictionary.Add(subItem.Key, subItem.Value);
                        }
                    }
                    else
                    {
                        var value = Convert.ToDouble(subItem.Value);

                        if (Math.Abs(value) <= TOLERANCE)
                        {
                            continue;
                        }

                        if (!activationDictionary.Keys.Contains(subItem.Key))
                        {
                            activationDictionary.Add(subItem.Key, subItem.Value);
                        }
                        else
                        {
                            activationDictionary[subItem.Key] = value.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
            }

            var result = (from c in activationDictionary
                          join m in machineListWithPermissions on c.Key equals m.Id.ToString()
                          select new MachineActivationDto
                          {
                              SortSeq = m.SortSeq,
                              MachineId = m.Id,
                              MachineName = m.Name,
                              Activation = Convert.ToDouble(c.Value)
                          }).ToList();

            return result;
        }

        /// <summary>
        /// 获取该角色已经获得权限的所有设备
        /// </summary>
        /// <returns></returns>
        private async Task<List<FlatMachineDto>> GetMachineDtoList()
        {
            var deviceGroupsAndMachinesWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var flatMachineDtoList =
                deviceGroupsAndMachinesWithPermissions.Machines.Where(
                    g => deviceGroupsAndMachinesWithPermissions.GrantedGroupIds.Contains(g.DeviceGroupId)).ToList();
            return flatMachineDtoList;
        }
 
    }
}