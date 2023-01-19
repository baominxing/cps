namespace Wimi.BtlCore.Shift.Dtos
{
    public class DeviceHistoryShiftInfoResult
    {
        public int DeviceId { get; set; }

        public decimal Duration { get; set; }

        public string EndDate { get; set; }

        public string EndTime { get; set; }

        public bool IsHistory { get; set; }

        public string ShiftName { get; set; }

        public int ShiftSolutionId { get; set; }

        public string ShiftSolutionName { get; set; }

        public string StartDate { get; set; }

        public string StartTime { get; set; }
    }
}
