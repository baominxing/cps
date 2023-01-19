namespace Wimi.BtlCore.ShiftDayTimeRange
{
    using System;

    public class ShiftDayTimeRange
    {
        public int MachineId { get; set; }

        public DateTime ShiftDay { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string BeginTime { get; set; }

        public string FinishTime { get; set; }

        public int MachineShiftDetailId { get; set; }

        public int ShiftSolutionId { get; set; }

        public string ShiftSolutionName { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public string ShiftItemName { get; set; }
    }
}