namespace Wimi.BtlCore.Shifts.Dto
{
    using System;

    public class DeviceGroupShiftSolutionInputDto
    {
        public int DeviceGroupId { get; set; }

        public DateTime EndTime { get; set; }

        public int ShiftSolutionId { get; set; }

        public DateTime StartTime { get; set; }
    }
}