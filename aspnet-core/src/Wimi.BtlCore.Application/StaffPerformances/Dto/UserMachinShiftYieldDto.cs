namespace Wimi.BtlCore.StaffPerformances.Dto
{
    using System;

    public class UserMachinShiftYieldDto
    {
        public string MachineName { get; set; }

        public string MachineShiftName { get; set; }

        public DateTime ShiftDate { get; set; }

        public string StaffShiftName { get; set; }

        public decimal SumYield { get; set; }

        public string UserName { get; set; }
    }
}