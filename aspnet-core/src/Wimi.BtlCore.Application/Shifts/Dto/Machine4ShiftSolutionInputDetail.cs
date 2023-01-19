using System.Collections.Generic;

namespace Wimi.BtlCore.Shifts.Dto
{
    using System;

    public class Machine4ShiftSolutionInputDetail
    {
        public DateTime EndTime { get; set; }

        public int Id { get; set; }

        public int ShiftSolutionId { get; set; }

        public DateTime StartTime { get; set; }

        public IEnumerable<int> MachineIds { get; set; }
    }
}