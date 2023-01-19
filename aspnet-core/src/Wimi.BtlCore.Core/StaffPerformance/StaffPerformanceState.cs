namespace Wimi.BtlCore.StaffPerformance
{
    public class StaffPerformanceState
    {
        public decimal DebugDuration { get; set; }

        public decimal FreeDuration { get; set; }

        public string Hexcode { get; set; }

        public bool IsByShift { get; set; }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public int MachinesShiftDetailId { get; set; }

        public decimal OfflineDuration { get; set; }

        public string ReasonCode { get; set; }

        public decimal ReasonDuration { get; set; }

        public string ReasonName { get; set; }

        public decimal RunDuration { get; set; }

        public string ShiftDay { get; set; }

        public decimal StopDuration { get; set; }

        public string SummaryDate { get; set; }

        public decimal TotalDuration { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }
    }
}