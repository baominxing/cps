namespace Wimi.BtlCore.Shifts.Dto
{
    using Abp.AutoMapper;
    using System;
    using Wimi.BtlCore.BasicData.Machines;

    [AutoMap(typeof(DeviceShiftSolutionInput))]
    public class DeviceShiftSolutionInputDto
    {
        public int DeviceId { get; set; }

        public DateTime EndTime { get; set; }

        public int ShiftSolutionId { get; set; }

        public DateTime StartTime { get; set; }
    }
}