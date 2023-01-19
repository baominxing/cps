using System;

namespace Wimi.BtlCore.BackgroundJobs.Workers
{
    using Abp.Configuration;
    using Abp.Domain.Repositories;
    using Abp.Domain.Uow;
    using Abp.ObjectMapping;
    using Abp.Threading;
    using Abp.Threading.Timers;
    using Hangfire;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Linq;
    using Wimi.BtlCore.BackgroundJobs.Dto;
    using Wimi.BtlCore.BasicData.Shifts;
    using Wimi.BtlCore.Configuration;
    using Wimi.BtlCore.Machines.Mongo;
    using Wimi.BtlCore.MongoException;
    using Wimi.BtlCore.ShiftDayTimeRange;

    /// <summary>
    /// 检查设备班次
    /// </summary>
    public class CheckMachineShiftTimelyWorker : WorkerBase
    {
        private readonly IBackgroundJobAppService backgroundJobAppService;
        private readonly MongoMachineManager mongoMachineManager;
        private readonly IShiftDayTimeRangeRepository shiftDayTimeRangeRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public CheckMachineShiftTimelyWorker(AbpTimer timer,
            MongoExceptionManager mongoExceptionManager,
            IBackgroundJobAppService backgroundJobAppService,
            MongoMachineManager mongoMachineManager,
            IUnitOfWorkManager unitOfWorkManager,
            IShiftDayTimeRangeRepository shiftDayTimeRangeRepository)
            : base(timer, mongoExceptionManager)
        {
            this.backgroundJobAppService = backgroundJobAppService;
            this.Timer.Period = OneMinute;
            this.mongoMachineManager = mongoMachineManager;
            this.unitOfWorkManager = unitOfWorkManager;
            this.shiftDayTimeRangeRepository = shiftDayTimeRangeRepository;
        }

        /// <summary>
        /// 每台设备遍历检查
        /// </summary>
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public void Execute()
        {
            var machineShiftDayTimeRanges = shiftDayTimeRangeRepository.ListMachineShiftDayTimeRange().ToList();
            var machines = mongoMachineManager.ListMongoMachine().ToList(); //未启用的设备 也需要排班，追溯数据会受影响

            var result = new List<MachineShiftDetailDto>();

            foreach (var machine in machines)
            {
                var shiftDetail = machineShiftDayTimeRanges.FirstOrDefault(m => m.MachineId == machine.MachineId);

                var item = new MachineShiftDetailDto(machine.MachineId, machine.MachinesShiftDetailId, machine.DateKey);

                if (shiftDetail != null)
                {
                    item.MachineId = machine.MachineId;
                    item.MachineShiftDetailId = shiftDetail.MachineShiftDetailId;
                    item.ShiftDay = shiftDetail.ShiftDay;
                    item.ShiftSolutionId = shiftDetail.ShiftSolutionId;
                    item.ShiftSolutionItemId = shiftDetail.ShiftSolutionItemId;
                    item.BeginTime = shiftDetail.BeginTime;
                    item.ShiftExtras = new MongoMachine.MachineShiftExtras
                    {
                        ShiftDay = shiftDetail.ShiftDay.ToString("yyyy-MM-dd"),
                        MachineShiftItemName = shiftDetail.ShiftItemName,
                        ShiftSolutionName = shiftDetail.ShiftSolutionName
                    };
                    item.ShiftDefail = new ShiftDefailInfo()
                    {
                        ShiftDay = shiftDetail.ShiftDay,
                        MachineShiftName = shiftDetail.ShiftItemName,
                        SolutionName = shiftDetail.ShiftSolutionName
                    };
                }

                if (item.ShiftDay.HasValue || !AppSettings.Shift.ShiftTimeOutside)
                {
                    result.Add(item);
                }
            }

            if (!result.Any()) return;
            this.UpdateMongoMachineInfo(result);
        }

        /// <summary>
        /// 逻辑处理入口
        /// </summary>
        protected override void DoWork()
        {
            if (!this.CheckJobIsEffective(typeof(CheckMachineShiftTimelyWorker))) return;

            BackgroundJob.Enqueue(() => Execute());
        }

        private void UpdateMongoMachineInfo(List<MachineShiftDetailDto> input)
        {
            var list = new List<WriteModel<MongoMachine>>();

            input.ForEach(
                m =>
                {
                    var filter = Builders<MongoMachine>.Filter.Where(s => s.MachineId == m.MachineId);
                    var update = Builders<MongoMachine>.Update.Set(s => s.MachinesShiftDetailId, m.MachineShiftDetailId)
                        .Set(s => s.ShiftExtras.ShiftDay, m.ShiftExtras.ShiftDay)
                        .Set(s => s.ShiftExtras.MachineShiftItemName, m.ShiftExtras.MachineShiftItemName)
                        .Set(s => s.ShiftExtras.ShiftSolutionName, m.ShiftExtras.ShiftSolutionName)
                        .Set(s => s.LastModificationTime, DateTime.Now.ToString("yyyyMMdd HH:mm:ss"))
                        .Set(s => s.DateKey, Convert.ToInt32(DateTime.Today.ToString("yyyyMMdd")));

                    list.Add(new UpdateManyModel<MongoMachine>(filter, update));
                });

            // 自然天优先
            if (!this.IfDaySwitch(input))
            {
                // 如果有班次切换
                this.IfShiftSwitch(input);
            }

            this.mongoMachineManager.BulkWriteMongoMachineShiftDetailId(list);
        }

        private bool IfDaySwitch(List<MachineShiftDetailDto> input)
        {
            var query = input.Where(t => t.DateKey < Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            if (!query.Any())
                return false;

            return AsyncHelper.RunSync(async () => await this.backgroundJobAppService.RuningStateSplitByHourNaturalDay(query.ToList()));
        }

        private void IfShiftSwitch(List<MachineShiftDetailDto> input)
        {
            var query = input.Where(t => t.PreviousMachinesShiftDetailId != t.MachineShiftDetailId && t.MachineShiftDetailId != 0);
            if (!query.Any())
                return;

            // 移除消息通知
            AsyncHelper.RunSync(async () => await this.backgroundJobAppService.RuningStateSplitByShift(query.ToList()));
        }
    }
}