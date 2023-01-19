using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Capacities;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Machines.Mongo;
using Wimi.BtlCore.Plan;
using Wimi.BtlCore.Plan.Repository;
using Wimi.BtlCore.ShiftDayTimeRange;
using Wimi.BtlCore.Trace.Dtos;

namespace Wimi.BtlCore.Trace.Manager
{
    public class TraceabilityManager : BtlCoreDomainServiceBase
    {
        private readonly IRepository<TraceCatalog, long> traceCatalogRepository;

        private readonly IRepository<TraceFlowSetting> traceFlowSettingRepository;

        private readonly IRepository<TraceRelatedMachine> traceRelatedMachineRepository;

        private readonly IRepository<Machine> machineRepository;

        private readonly IRepository<TraceFlowRecord, long> traceFlowRecordRepository;

        private readonly IPlanRepository planRepository;

        private readonly IRepository<Capacity> capacityRepository;

        private readonly MongoMachineManager mongoMachineManager;

        private readonly IShiftDayTimeRangeRepository shiftDayTimeRangeRepository;

        private readonly IRepository<ProcessPlan> processPlanRepository;

        public TraceabilityManager(
            IRepository<TraceCatalog, long> traceCatalogRepository,
            IRepository<TraceFlowSetting> traceFlowSettingRepository,
            IRepository<TraceRelatedMachine> traceRelatedMachineRepository,
            IRepository<Machine> machineRepository,
            IRepository<TraceFlowRecord, long> traceFlowRecordRepository,
            IPlanRepository planRepository,
            IRepository<Capacity> capacityRepository,
            MongoMachineManager mongoMachineManager,
            IShiftDayTimeRangeRepository shiftDayTimeRangeRepository,
            IRepository<ProcessPlan> processPlanRepository)
        {
            this.traceCatalogRepository = traceCatalogRepository;
            this.traceFlowSettingRepository = traceFlowSettingRepository;
            this.traceRelatedMachineRepository = traceRelatedMachineRepository;
            this.machineRepository = machineRepository;
            this.traceFlowRecordRepository = traceFlowRecordRepository;
            this.planRepository = planRepository;
            this.capacityRepository = capacityRepository;
            this.mongoMachineManager = mongoMachineManager;
            this.shiftDayTimeRangeRepository = shiftDayTimeRangeRepository;
            this.processPlanRepository = processPlanRepository;
        }

        public async Task<int> OfflinePart(OfflinePartInputDto input)
        {
            var traceCatalog = await this.traceCatalogRepository.FirstOrDefaultAsync(x => x.PartNo == input.PartNo);

            var traceFlowSetting = await this.traceFlowSettingRepository
                .FirstOrDefaultAsync(x => x.DeviceGroupId == traceCatalog.DeviceGroupId && x.StationType == StationType.FinalInspection);
            if (traceFlowSetting == null)
            {
                throw new UserFriendlyException(L("CantFindFinalSpcFlow"));
            }

            var traceRelatedMachine = await this.traceRelatedMachineRepository.FirstOrDefaultAsync(x => x.TraceFlowSettingId == traceFlowSetting.Id);
            if (traceRelatedMachine == null)
            {
                throw new UserFriendlyException(L("CantFindFinalSpcFlowRelatedMachine"));
            }

            var machine = await this.machineRepository.FirstOrDefaultAsync(x => x.Id == traceRelatedMachine.MachineId);

            this.DeatWithLeftFlow(input.PartNo, traceFlowSetting, machine);

            //插入追溯记录
            this.InsertTraceRecord(input.PartNo, machine, input.Qualified, traceFlowSetting);

            //更新上下线结果
            this.UpdateTraceCatalogOffline(traceCatalog, input.Qualified, DateTime.Now);

            //处理产量
            await this.HandlerLineCapacity(input.PartNo, traceCatalog, machine, input.Qualified);

            //处理计划
            planRepository.HandlerLinePlan(traceCatalog, input.Qualified);

            return machine.Id;
        }

        private void DeatWithLeftFlow(string partNo, TraceFlowSetting setting, Machine machine)
        {
            if (setting.TriggerEndFlowStyle == TriggerEndFlowStyle.PrevFlow)
            {
                this.UpdateLastFlowLeftTime(partNo);
            }
            if (setting.TriggerEndFlowStyle == TriggerEndFlowStyle.SelfFlow)
            {
                this.UpdateSelfLastFlowLeftTime(setting, machine);
            }
            if (setting.TriggerEndFlowStyle == TriggerEndFlowStyle.SelfAndPrevFlow)
            {
                this.UpdateLastFlowLeftTime(partNo);
                this.UpdateSelfLastFlowLeftTime(setting, machine);
            }
        }

        private void UpdateLastFlowLeftTime(string partNo)
        {
            var lastFlow = this.traceFlowRecordRepository.GetAll().Where(x => x.PartNo == partNo && x.LeftTime == null)
                .OrderByDescending(x => x.EntryTime).FirstOrDefault();
            if (lastFlow != null)
            {
                lastFlow.LeftTime = DateTime.Now;
                lastFlow.State = FlowState.Done;
                this.traceFlowRecordRepository.Update(lastFlow);
            }
        }

        private void UpdateSelfLastFlowLeftTime(TraceFlowSetting setting, Machine machine)
        {
            var lastFlow = this.traceFlowRecordRepository.GetAll().Where(x => x.TraceFlowSettingId == setting.Id && x.MachineCode == machine.Code && x.State == FlowState.Wip)
                .OrderByDescending(x => x.EntryTime).FirstOrDefault();
            if (lastFlow != null)
            {
                lastFlow.LeftTime = DateTime.Now;
                lastFlow.State = FlowState.Done;
                this.traceFlowRecordRepository.Update(lastFlow);
            }
        }

        private void InsertTraceRecord(string partNo, Machine machine, bool qualified, TraceFlowSetting setting)
        {
            var tag = qualified ? FlowTag.Qualified : FlowTag.UnQualified;
            this.traceFlowRecordRepository.Insert(new TraceFlowRecord
            {
                PartNo = partNo,
                FlowCode = setting.Code,
                FlowDisplayName = setting.DisplayName,
                TraceFlowSettingId = setting.Id,
                MachineCode = machine.Code,
                MachineId = machine.Id,
                Station = string.Empty,
                EntryTime = DateTime.Now,
                LeftTime = DateTime.Now,
                State = FlowState.Done,
                Tag = tag,
                ExtensionData = null,
                UserId = 2
            });
        }

        private void UpdateTraceCatalogOffline(TraceCatalog traceCatalog, bool qualified, DateTime offlineTime)
        {
            traceCatalog.OfflineTime = offlineTime;
            traceCatalog.Qualified = qualified;
            this.traceCatalogRepository.Update(traceCatalog);
        }

        private async Task HandlerLineCapacity(string partNo, TraceCatalog traceCatalog, Machine machine, bool qualified)
        {
            //检查是否存在非返工的产量记录
            var isExistNoReworkCapacity = await this.capacityRepository.GetAll().AnyAsync(c => c.PartNo == partNo && c.Tag != "1" && c.IsLineOutput == true);

            //不存在则新增产量
            if (!isExistNoReworkCapacity)
            {
                var mongoMachine = this.mongoMachineManager.GetMachineInfoFromMongo(machine.Code);

                var shift = this.shiftDayTimeRangeRepository.GetMachineCurrentShiftDetail(mongoMachine.MachinesShiftDetailId);

                var plan = await this.processPlanRepository.FirstOrDefaultAsync(p => p.DeviceGroupId == traceCatalog.DeviceGroupId && p.Status == EnumPlanStatus.InProgress);

                var lineCapacity = new Capacity()
                {
                    MachineId = machine.Id,
                    MachineCode = machine.Code,
                    Yield = 1,
                    AccumulateCount = 1,
                    OriginalCount = 1,
                    Rate = 1,
                    StartTime = DateTime.Now,
                    Duration = 0,
                    IsValid = true,
                    PartNo = partNo,
                    IsLineOutput = true,
                    Tag = "0",
                    Qualified = qualified,
                    MachinesShiftDetailId = shift.MachineShiftDetailId,
                    ShiftDetail = new ShiftDefailInfo { MachineShiftName = shift.ShiftDetail_MachineShiftName, SolutionName = shift.ShiftDetail_SolutionName, ShiftDay = shift.MachineShiftDetailId == 0 ? null : shift.ShiftDetail_ShiftDay },
                    PlanId = plan?.Id ?? 0,
                    PlanName = plan != null ? plan.PlanName : "",
                    PlanAmount = plan?.PlanAmount ?? 0,
                    ProductName = plan != null ? plan.ProductName : "",
                    ProductId = plan?.ProductId ?? 0
                };
                await this.capacityRepository.InsertAsync(lineCapacity);
            }
        }


    }
}
