using System;
using System.Collections.Generic;

namespace Wimi.BtlCore.Shifts.Dto
{
    public class MultiMachineShiftInputDto
    {
        public MultiMachineShiftInputDto()
        {
            this.MachineIdList = new List<int>();
        }

        public int ShiftSolutionId { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime StartTime { get; set; }

        public List<int> MachineIdList { get; set; }
    }
}
