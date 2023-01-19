namespace Wimi.BtlCore.StaffPerformances.Dto
{
    using System.Collections.Generic;

    public class AllMachinesRquestDto
    {
        public AllMachinesRquestDto()
        {
            this.MachineIds = new int[] { };
        }

        public IEnumerable<int> MachineIds { get; set; }
    }
}