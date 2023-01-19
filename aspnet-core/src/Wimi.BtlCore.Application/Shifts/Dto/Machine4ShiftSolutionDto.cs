namespace Wimi.BtlCore.Shifts.Dto
{
    using System.Collections.Generic;

    public class Machine4ShiftSolutionDto
    {
        public List<Machine4ShiftSolutionDetail> Machine4ShiftSolutionDetail { get; set; }

        public int MachineId { get; set; }
    }
}