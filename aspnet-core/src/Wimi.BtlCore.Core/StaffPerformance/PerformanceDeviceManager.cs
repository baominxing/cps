namespace Wimi.BtlCore.StaffPerformance
{
    using Abp.Domain.Repositories;
    using Abp.Events.Bus;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.BasicData.States;
    using Wimi.BtlCore.Extensions;
    using Wimi.BtlCore.KafkaProducer;
    using Wimi.BtlCore.Machines.Mongo;
    using Wimi.BtlCore.ShiftDayTimeRange;
    using Wimi.BtlCore.States;
    using Wimi.BtlCore.States.Mongo;

    public class PerformanceDeviceManager : BtlCoreDomainServiceBase, IPerformanceDeviceManager
    {
        private readonly IEventBus eventBus;

        private readonly IRepository<PerformancePersonnelOnDevice> performancePersonnelOnDeviceRepository;
        private readonly MongoStateManager mongoStateManager;
        private readonly MongoMachineManager mongoMachineManager;
        private readonly IRepository<Machine> machineRepository;
        private readonly IShiftDayTimeRangeRepository shiftDayTimeRangeRepository;
        private readonly StateManager stateManager;
        private readonly IKafkaProducerManager kafkaProducerManager;
        private readonly IRepository<State, long> stateRepository;

        public PerformanceDeviceManager(
            IRepository<PerformancePersonnelOnDevice> performancePersonnelOnDeviceRepository, 
            IEventBus eventBus,
             MongoStateManager mongoStateManager,
             MongoMachineManager mongoMachineManager,
             IRepository<Machine> machineRepository, IRepository<State, long> stateRepository,
             StateManager stateManager,
             IKafkaProducerManager kafkaProducerManager,
             IShiftDayTimeRangeRepository shiftDayTimeRangeRepository)
        {
            this.performancePersonnelOnDeviceRepository = performancePersonnelOnDeviceRepository;
            this.eventBus = eventBus;
            this.mongoStateManager = mongoStateManager;
            this.mongoMachineManager = mongoMachineManager;
            this.machineRepository = machineRepository;
            this.stateManager = stateManager;
            this.kafkaProducerManager = kafkaProducerManager;
            this.shiftDayTimeRangeRepository = shiftDayTimeRangeRepository;
            this.stateRepository = stateRepository;
        }

        public async Task Offline(int machineId, long userId)
        {
            await this.performancePersonnelOnDeviceRepository.DeleteAsync(p => p.MachineId == machineId);

             HandleMongoState(machineId, 0, 0);

            this.eventBus.Trigger(new PersonOfflineEventData(machineId, userId));
        }

        public async Task Online(int machineId, long userId, int shiftId)
        {
            var entity = new PerformancePersonnelOnDevice
                             {
                                 MachineId = machineId, 
                                 UserId = userId, 
                                 OnlineDate = DateTime.Now
                             };

            // 插入数据
            await this.performancePersonnelOnDeviceRepository.InsertAsync(entity);

            //处理Mongo数据
            HandleMongoState(machineId, userId, shiftId);

            // 获取usershiftId 对应的ShiftItemName 
            var shiftItemName = this.shiftDayTimeRangeRepository.GetShiftItemName(shiftId);

            // 处理日志
            this.eventBus.Trigger(new PersonOnlineEventData(machineId, userId, shiftId, entity.OnlineDate, shiftItemName));

            //处理Mongo数据
            HandleMongoState(machineId, userId, shiftId);
        }


        private void HandleMongoState(int machineId, long userId, int shiftId)
        {
            var query = this.stateRepository.GetAll()
                .Where(s => s.MachineId == machineId && !s.EndTime.HasValue).ToList();
            foreach (var item in query)
            {
                var state = new State
                {
                    DmpId = item.DmpId,
                    Code = item.Code,
                    DateKey = item.DateKey,
                    MachineCode = item.MachineCode,
                    MachineId = item.MachineId,
                    MachinesShiftDetailId = item.MachinesShiftDetailId,
                    Memo = item.Memo,
                    MongoCreationTime = item.MongoCreationTime,
                    Name = item.Name,
                    OrderId = item.OrderId,
                    PartNo = item.PartNo,
                    ProcessId = item.ProcessId,
                    ProductId = item.ProductId,
                    ProgramName = item.ProgramName,
                    ShiftDetail = new BasicData.Shifts.ShiftDefailInfo
                    {
                        MachineShiftName = item.ShiftDetail.MachineShiftName,
                        ShiftDay = item.ShiftDetail.ShiftDay,
                        SolutionName = item.ShiftDetail.SolutionName,
                        StaffShiftName = item.ShiftDetail.StaffShiftName
                    }
                };
                    
                //ObjectMapper.Map<tate>(item);
                item.EndShift(DateTime.Now);

                this.stateRepository.Update(item);
                this.CurrentUnitOfWork.SaveChanges();

                state.StartStaffOnline(userId, shiftId);

                this.stateRepository.Insert(state);
                this.CurrentUnitOfWork.SaveChanges();

                //更新Mongo中设备的最新状态
                this.mongoMachineManager.UpdateMongoMachineState(machineId, new MongoMachine.MachineState { Code = state.Code, DmpId = state.DmpId.ToString(), CreationTime = DateTime.Now.ToMongoDateTime() });

            }
        }

    }
}