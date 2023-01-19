using System.Globalization;
using Abp.Domain.Uow;

namespace Wimi.BtlCore.BackgroundJobs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Abp.AutoMapper;
    using Abp.Collections.Extensions;
    using Abp.Dependency;
    using Abp.Domain.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Wimi.BtlCore.BackgroundJobs.Dto;
    using Wimi.BtlCore.BackgroundJobs.Workers;
    using Wimi.BtlCore.BasicData.Machines.Repository;
    using Wimi.BtlCore.BasicData.Shifts;
    using Wimi.BtlCore.BasicData.States;
    using Wimi.BtlCore.Extensions;
    using Wimi.BtlCore.Machines.Mongo;
    using Wimi.BtlCore.ShiftDayTimeRange;

    public class BackgroundJobAppService : BtlCoreAppServiceBase, IBackgroundJobAppService
    {
        private readonly IRepository<State, long> stateRepository;
        private readonly IStateRepository stateSqlRepository;
        private readonly IIocResolver iocResolver;
        private readonly MongoMachineManager mongoMachineManager;
        private readonly IRepository<ShiftSolutionItem> shiftItemRepository;
        private readonly IRepository<MachinesShiftDetail> machinesShiftDetailRepository;
        private readonly IShiftDayTimeRangeRepository shiftDayTimeRangeRepository;
        private readonly SyncMongoDataWorker syncMongoDataWorker;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public BackgroundJobAppService(IRepository<State, long> stateRepository,
            IStateRepository stateSqlRepository,
            IIocResolver iocResolver,
            MongoMachineManager mongoMachineManager,
            IRepository<ShiftSolutionItem> shiftItemRepository,
            IShiftDayTimeRangeRepository shiftDayTimeRangeRepository,
            IRepository<MachinesShiftDetail> machinesShiftDetailRepository,
            SyncMongoDataWorker syncMongoDataWorker,
            IUnitOfWorkManager unitOfWorkManager)
        {
            this.stateRepository = stateRepository;
            this.stateSqlRepository = stateSqlRepository;
            this.iocResolver = iocResolver;
            this.mongoMachineManager = mongoMachineManager;
            this.shiftItemRepository = shiftItemRepository;
            this.shiftDayTimeRangeRepository = shiftDayTimeRangeRepository;
            this.machinesShiftDetailRepository = machinesShiftDetailRepository;
            this.syncMongoDataWorker = syncMongoDataWorker;
            this.unitOfWorkManager = unitOfWorkManager;
        }


        /// <summary>
        /// 需要修复--每台设备会切分出来大于1条的记录，本次切分时要自动修复该记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task RuningStateSplitByShift(IEnumerable<MachineShiftDetailDto> input)
        {
            this.syncMongoDataWorker.SyncState();

            using (var unitofWork = this.unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                var result = this.stateRepository.GetAll().Where(s => !s.EndTime.HasValue).GroupBy(t => t.MachineId).Select(t => new { MachineId = t.Key, List = t.ToList() });

                foreach (var item in result)
                {
                    var recentlyState = item.List.OrderByDescending(t => t.StartTime).FirstOrDefault();
                    var machine = input.FirstOrDefault(t => t.MachineId == item.MachineId);
                    if (machine == null) continue;

                    foreach (var entity in item.List)
                    {
                        var endTime = !string.IsNullOrEmpty(machine.BeginTime) ? Convert.ToDateTime(machine.BeginTime) : DateTime.Now;

                        Logger.Debug($@"设备:[{entity.MachineCode}] 班次:[{machine.ShiftDefail.MachineShiftName}] 工厂日：[{machine.ShiftDefail.ShiftDay}],共[{item.List.Count}--【{item.List.Select(t => t.Id).JoinAsString(",")}】]条记录要处理，其中[{recentlyState?.Id}]复制会新增");

                        // 如果出现多笔记录，结束的这笔记录的EndTime 需要修正为连续数据
                        if (item.List.Count > 1 && entity.Id != recentlyState?.Id)
                        {
                            // 距离该记录最近的记录
                            var nearbyEntity = this.stateRepository.GetAll().Where(t => t.MachineId == entity.MachineId && t.StartTime > entity.StartTime).Min(t => t.StartTime);
                            endTime = nearbyEntity ?? endTime;
                            Logger.Debug($@"设备:[{entity.MachineCode}],记录[{entity.Id}] 修正结束时间为:[{endTime.ToString("yyyy-MM-dd HH:ss:MM")}] ");
                        }

                        //这里需要拷贝一份数据，而不是同一个实体上的映射
                        var state = NClone.Clone.ObjectGraph(entity);
                        entity.EndShift(endTime);

                        await this.stateRepository.UpdateAsync(entity);

                        //如果一个设备有多条EndTime == null记录，只可以将最新一笔复制插入，绝对不能全部复制插入
                        if (entity.Id == recentlyState?.Id)
                        {
                            var shiftDetailInfo = await this.stateSqlRepository.GetShiftDefailInfo(machine.MachineShiftDetailId);
                            state.StartCurrentShift(machine.MachineShiftDetailId, endTime, shiftDetailInfo);

                            await this.stateRepository.InsertAndGetIdAsync(state);

                            //更新Mongo上的dmpId
                            this.mongoMachineManager.UpdateMongoMachineState(machine.MachineId, new MongoMachine.MachineState { Code = state.Code, DmpId = state.DmpId.ToString(), CreationTime = DateTime.Now.ToMongoDateTime() });
                        }
                    }
                }

                unitofWork.Complete();
            }          
        }

        public async Task<bool> RuningStateSplitByHourNaturalDay(IEnumerable<MachineShiftDetailDto> input)
        {
            var returnValue = false;
            this.syncMongoDataWorker.SyncState();

            using (var unitofWork = this.unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                foreach (var item in input)
                {
                    this.mongoMachineManager.UpdateDateKey(item.MachineId);

                    var endTime = DateTime.ParseExact($"{item.DateKey}", "yyyyMMdd", CultureInfo.CurrentCulture).AddDays(1);

                    var query = await this.stateRepository.GetAll().Where(s => s.MachineId == item.MachineId && !s.EndTime.HasValue && s.StartTime < endTime).ToListAsync();

                    foreach (var state in query)
                    {
                        returnValue = true;
                        var newState = NClone.Clone.ObjectGraph(state);
                        state.EndShift(endTime);
                        await this.stateRepository.UpdateAsync(state);

                        var naturalDayShift = shiftDayTimeRangeRepository.GetMachineNaturalDayShift(
                               DateTime.ParseExact($"{item.DateKey}", "yyyyMMdd", CultureInfo.CurrentCulture).AddDays(1), item.MachineId);

                        var newStateShiftDetail = NClone.Clone.ObjectGraph(state.ShiftDetail);

                        if (naturalDayShift != null && naturalDayShift.MachineShiftDetailId != 0 && naturalDayShift.MachineShiftDetailId != state.MachinesShiftDetailId)
                        {
                            //切天同时切班
                            newStateShiftDetail = await this.stateSqlRepository.GetShiftDefailInfo(naturalDayShift.MachineShiftDetailId);
                        }

                        newState.StartCurrentNaturalDay(newStateShiftDetail, naturalDayShift?.MachineShiftDetailId ?? null);
                        if (newStateShiftDetail == null)
                        {
                            newState.ShiftDetail.ShiftDay = naturalDayShift.ShiftDay;
                        }

                        await this.stateRepository.InsertAsync(newState);

                        //更新Mongo上的dmpId
                        this.mongoMachineManager.UpdateMongoMachineState(item.MachineId, new MongoMachine.MachineState { Code = state.Code, DmpId = newState.DmpId.ToString(), CreationTime = DateTime.Now.ToMongoDateTime() });
                    }
                }

                unitofWork.Complete();
            }

            return returnValue;
        }

        public void RunningDmpJobAboutShift()
        {
            iocResolver.Resolve<CheckMachineShiftTimelyWorker>().Execute();
            iocResolver.Resolve<SyncMongoDataWorker>().Execute();
        }

        private ShiftInfosDto GetShiftInfosByDetailId(int machineShiftDetailId)
        {
            var shiftDetail = this.machinesShiftDetailRepository.FirstOrDefault(machineShiftDetailId);
            if (shiftDetail == null) return null;

            using (this.CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var shiftItems = this.shiftItemRepository.FirstOrDefault(shiftDetail.ShiftSolutionItemId);

                DateTime startTime;
                if (shiftItems.IsNextDay && shiftItems.StartTime < shiftItems.EndTime)
                {
                    startTime = shiftDetail.ShiftDay.AddDays(1).AddHours(shiftItems.StartTime.Hour).AddMinutes(shiftItems.StartTime.Minute);
                }
                else
                {
                    startTime = shiftDetail.ShiftDay.AddHours(shiftItems.StartTime.Hour).AddMinutes(shiftItems.StartTime.Minute);
                }

                var infos = new ShiftInfosDto
                {
                    MachineSfhitDetailId = machineShiftDetailId,
                    MachineShiftName = shiftItems.Name,
                    ShiftDay = shiftDetail.ShiftDay,
                    ShiftStartTime = startTime
                };
                return infos;
            }
        }
    }
}