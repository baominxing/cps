using System.Collections.Generic;

namespace Wimi.BtlCore.Shifts.Dto
{
    public class Machine4ShiftSolutionInputDto
    {
        public Machine4ShiftSolutionInputDto()
        {
            this.Machine4ShiftSolutionInputDetail = new List<Machine4ShiftSolutionInputDetail>();
        }

        public int Id { get; set; }

        public List<Machine4ShiftSolutionInputDetail> Machine4ShiftSolutionInputDetail { get; set; }
    }

}
