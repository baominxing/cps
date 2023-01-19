namespace Wimi.BtlCore.StaffPerformances.Dto
{
    using System;

    public class UserMachineYieldDto
    {
        public string DateStr { get; set; }

        public string GroupBy { get; set; }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public string MachineShiftName { get; set; }

        public string MachineShiftStr { get; set; }

        public DateTime? MachineStartDate { get; set; }

        public string MonthStr { get; set; }

        public string QuarterStr { get; set; }

        public DateTime ShiftDate { get; set; }

        public string StaffShiftName { get; set; }

        public string StaffShiftStr { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string WeekStr { get; set; }

        public string YearStr { get; set; }

        public decimal Yield { get; set; }
    }
}