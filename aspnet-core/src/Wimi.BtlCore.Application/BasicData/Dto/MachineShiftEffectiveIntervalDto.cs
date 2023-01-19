using System;
using Abp.AutoMapper;
using Wimi.BtlCore.BasicData.Shifts;

namespace Wimi.BtlCore.BasicData.Dto
{
    [AutoMap(typeof(MachineShiftEffectiveInterval))]
    public class MachineShiftEffectiveIntervalDto
    {
        public string MachineName { get; set; }

        public int MachineId { get; set; }

        public string ShiftSolutionName { get; set; }

        public int ShiftSolutionId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public double ExpiryDay => Math.Abs((DateTime.Today - this.EndTime).TotalDays);
    }
}