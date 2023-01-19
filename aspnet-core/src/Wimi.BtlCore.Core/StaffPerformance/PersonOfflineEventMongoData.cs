namespace Wimi.BtlCore.StaffPerformance
{
    using Abp.Events.Bus;

    public class PersonOfflineEventMongoData : EventData
    {
        public PersonOfflineEventMongoData(int machineId)
        {
            this.MachineId = machineId;
        }

        public int MachineId { get; set; }
    }
}