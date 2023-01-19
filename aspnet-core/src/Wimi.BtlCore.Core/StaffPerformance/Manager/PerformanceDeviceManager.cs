
namespace Wimi.BtlCore.StaffPerformance.Manager
{
    using Abp;
    using Abp.Domain.Repositories;
    using Abp.Events.Bus;
    using MongoDB.Bson;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.KafkaProducer;
    using Wimi.BtlCore.Machines.Mongo;
    using Wimi.BtlCore.ShiftDayTimeRange;
    using Wimi.BtlCore.States.Mongo;

    public class PerformanceDeviceManager : BtlCoreDomainServiceBase, IPerformanceDeviceManager
    {
        private readonly IEventBus eventBus;

        private readonly IRepository<PerformancePersonnelOnDevice> performancePersonnelOnDeviceRepository;
        private readonly MongoStateManager mongoStateManager;
        private readonly MongoMachineManager mongoMachineManager;
        private readonly IRepository<Machine> machineRepository;
        private readonly IShiftDayTimeRangeRepository shiftDayTimeRangeRepository;
        private readonly IKafkaProducerManager kafkaProducerManager;

        public PerformanceDeviceManager(
            IRepository<PerformancePersonnelOnDevice> performancePersonnelOnDeviceRepository,
            IEventBus eventBus,
             MongoStateManager mongoStateManager,
             MongoMachineManager mongoMachineManager,
             IRepository<Machine> machineRepository,
             IKafkaProducerManager kafkaProducerManager,
             IShiftDayTimeRangeRepository shiftDayTimeRangeRepository)
        {
            this.performancePersonnelOnDeviceRepository = performancePersonnelOnDeviceRepository;
            this.eventBus = eventBus;
            this.mongoStateManager = mongoStateManager;
            this.mongoMachineManager = mongoMachineManager;
            this.machineRepository = machineRepository;
            this.kafkaProducerManager = kafkaProducerManager;
            this.shiftDayTimeRangeRepository = shiftDayTimeRangeRepository;
        }

        public async Task Offline(int machineId, long userId)
        {
            await this.performancePersonnelOnDeviceRepository.DeleteAsync(p => p.MachineId == machineId);

            HandleMongoState(machineId, 0, 0);

            this.eventBus.Trigger(new PersonOfflineEventData(machineId, userId));
        }

        public async Task Online(int machineId, long userId, int shiftId)
        {
            var entity = new PerformancePersonnelOnDevice { MachineId = machineId, UserId = userId, OnlineDate = DateTime.Now };

            // 插入数据
            await this.performancePersonnelOnDeviceRepository.InsertAsync(entity);

            //处理Mongo数据
            HandleMongoState(machineId, userId, shiftId);

            // 获取usershiftId 对应的ShiftItemName 
            var shiftItemName = this.shiftDayTimeRangeRepository.GetShiftItemName(shiftId);

            // 处理日志
            this.eventBus.Trigger(new PersonOnlineEventData(machineId, userId, shiftId, entity.OnlineDate, shiftItemName));
        }


        private void HandleMongoState(int machineId, long userId, int shiftId)
        {
            var machineCode = machineRepository.Get(machineId)?.Code;

            var targetMchine = mongoMachineManager.GetMongoMachineByCode(machineCode);

            if (targetMchine == null) return;

            var lastrecord = targetMchine.State;

            if (lastrecord == null) return;

            var creationTime = DateTime.Now.ToString("yyyyMMddHHmmssffff");

            var state = new MongoState()
            {
                Code = lastrecord.Code,
                DateKey = Convert.ToInt32(creationTime.Substring(0, 8)),
                DmpId = Guid.NewGuid().ToString(),
                Id = new ObjectId(),
                MachineCode = machineCode,
                MachineId = machineId,
                CreationTime = creationTime,
                MachineShiftItemName = targetMchine.ShiftExtras.MachineShiftItemName,
                MachinesShiftDetailId = targetMchine.MachinesShiftDetailId,
                OrderId = targetMchine.OrderId,
                PartNo = targetMchine.PartNo,
                PreDmpId = targetMchine.State.DmpId,
                ProcessId = targetMchine.ProcessId,
                ProductId = targetMchine.ProductId,
                ProgramName = targetMchine.ProgramName,
                ShiftDay = targetMchine.ShiftExtras.ShiftDay,
                ShiftSolutionName = targetMchine.ShiftExtras.ShiftSolutionName,
                StaffShiftItemName = targetMchine.ShiftExtras.StaffShiftItemName,
                UserId = targetMchine.UserId == 0 ? userId : 0,
                UserShiftDetailId = targetMchine.UserShiftDetailId == 0 ? shiftId : 0
            };

            //更新Mongo中设备的最新状态
            this.mongoMachineManager.UpdateMongoMachineState(machineId, new MongoMachine.MachineState { Code = state.Code, DmpId = state.DmpId, CreationTime = state.CreationTime });

            //推送状态
            var mongoData = new NameValue<string>("State", JsonConvert.SerializeObject(state));

            this.kafkaProducerManager.Produce(mongoData);

        }
    }
}