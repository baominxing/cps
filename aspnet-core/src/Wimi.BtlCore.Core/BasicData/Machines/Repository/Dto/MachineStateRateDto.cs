namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class MachineStateRateDto
    {
        public string Mac_no { get; set; }

        public int SortSeq { get; set; }

        public long SummaryId { get; set; }

        public string SummaryName { get; set; }

        public string SummaryDate { get; set; }

        public decimal StopDuration { get; set; }

        public decimal RunDuration { get; set; }

        public decimal FreeDuration { get; set; }

        public decimal OfflineDuration { get; set; }

        public decimal DebugDuration { get; set; }

        public decimal TotalDuration { get; set; }

        public decimal StopDurationRate { get; set; }

        public decimal RunDurationRate { get; set; }

        public decimal FreeDurationRate { get; set; }

        public decimal OfflineDurationRate { get; set; }

        public decimal DebugDurationRate { get; set; }

        public string Code { get; set; }

        public decimal Duration { get; set; }
    }
}
