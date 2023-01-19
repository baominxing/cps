namespace Wimi.BtlCore.StaffPerformance
{
    using Abp.Events.Bus;

    public class PersonOfflineEventData : EventData
    {
        public PersonOfflineEventData(int deviceId, long userId)
        {
            this.DeviceId = deviceId;
            this.UserId = userId;
        }

        public int DeviceId { get; private set; }

        public long UserId { get; private set; }
    }
}