namespace Wimi.BtlCore.StaffPerformance
{
    public class StaffPerformanceStateRate
    {
        public decimal DebugDurationRate { get; set; }

        public decimal FreeDurationRate { get; set; }

        public string Hexcode { get; set; }

        public bool IsByShift { get; set; }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public int MachinesShiftDetailId { get; set; }

        public decimal OfflineDurationRate { get; set; }

        public string ReasonCode { get; set; }

        public decimal ReasonDuration { get; set; }

        public string ReasonName { get; set; }

        public decimal ReasonRate { get; set; }

        public decimal RunDurationRate { get; set; }

        public string ShiftDay { get; set; }

        public decimal StopDurationRate { get; set; }

        public string SummaryDate { get; set; }

        public decimal TotalDuration { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }
    }
}