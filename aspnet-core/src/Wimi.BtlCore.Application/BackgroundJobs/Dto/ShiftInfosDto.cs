using System;

namespace Wimi.BtlCore.BackgroundJobs.Dto
{
    public class ShiftInfosDto
    {

        public int MachineSfhitDetailId { get; set; }

        public string MachineShiftName { get; set; }

        public DateTime? ShiftDay { get; set; }

        public DateTime ShiftStartTime { get; set; }

        public string SolutionName { get; set; }
    }
}
