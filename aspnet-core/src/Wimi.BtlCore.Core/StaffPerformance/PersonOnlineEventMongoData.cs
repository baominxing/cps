namespace Wimi.BtlCore.StaffPerformance
{
    using Abp.Events.Bus;

    public class PersonOnlineEventMongoData : EventData
    {
        public PersonOnlineEventMongoData(int machineId, int shiftId, long userId)
        {
            this.MachineId = machineId;
            this.ShiftId = shiftId;
            this.UserId = userId;
        }

        public int MachineId { get; set; }

        public int ShiftId { get; set; }

        public long UserId { get; set; }
    }
}