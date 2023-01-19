namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class MachineStateDto
    {
        public decimal DebugDuration { get; set; }

        public decimal FreeDuration { get; set; }

        public long MachineId { get; set; }

        public string MachineName { get; set; }

        public decimal OfflineDuration { get; set; }

        public decimal RunDuration { get; set; }

        public decimal StopDuration { get; set; }

        public string SummaryDate { get; set; }

        public decimal TotalDuration { get; set; }
    }
}
