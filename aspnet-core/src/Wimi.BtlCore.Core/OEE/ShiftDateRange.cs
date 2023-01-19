namespace Wimi.BtlCore.OEE
{
    using System;

    public class ShiftDateRange
    {
        public DateTime Date { get; set; }

        public string ShiftDay { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}