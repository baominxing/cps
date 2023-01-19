namespace Wimi.BtlCore.StaffPerformance
{
    using System;

    using Abp.Events.Bus;

    public class PersonOnlineEventData : EventData
    {
        public PersonOnlineEventData(int deviceId, long userId, int shiftId, DateTime onlineDate, string shiftItemName)
        {
            this.DeviceId = deviceId;
            this.UserId = userId;
            this.ShiftId = shiftId;
            this.OnlineDate = onlineDate;
            this.ShiftItemName = shiftItemName;
        }

        public int DeviceId { get; private set; }

        public DateTime OnlineDate { get; private set; }

        public int ShiftId { get; private set; }

        public long UserId { get; private set; }

        public string ShiftItemName { get; set; }
    }
}