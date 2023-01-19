using System;

namespace Wimi.BtlCore.BasicData.Machines
{
    public class DeviceShiftSolutionInput
    {
        public int DeviceId { get; set; }

        public DateTime EndTime { get; set; }

        public int ShiftSolutionId { get; set; }

        public DateTime StartTime { get; set; }

        public long CreatorUserId { get; set; }
    }
}
