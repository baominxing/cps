using Wimi.BtlCore.BasicData.Machines;

namespace Wimi.BtlCore.Dashboard.Dtos
{
    public class MachineStatisticDataDto
    {
        public decimal? DebugRate { get; set; }

        public decimal? FreeRate { get; set; }

        public EnumMachineState? MachineStatus { get; set; }

        public decimal? OfflineRate { get; set; }

        public decimal? RunRate { get; set; }

        public decimal? StopRate { get; set; }

        public string SummaryDate { get; set; }

        public decimal? Yield { get; set; }
    }
}
