using System;

namespace Wimi.BtlCore.BasicData.Shifts.Manager.Dto
{
    public class GetMachineShiftSolutionsDto
    {
        public string MachineShiftSolutionId { get; set; }

        public string MachineShiftSolutionName { get; set; }

        public string MachineId { get; set; }

        public string MachineName { get; set; }
    }

    public class GetSummaryDateDto
    {
        public string SummaryDate { get; set; }
    }

    public class CorrectQueryDateDto
    {
        public string ShiftId { get; set; }

        public string ShiftName { get; set; }

        public string ShiftDay { get; set; }

        public string ShiftWeek { get; set; }

        public string ShiftMonth { get; set; }

        public string ShiftYear { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
