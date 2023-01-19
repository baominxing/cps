namespace Wimi.BtlCore.Dashboard.Dtos
{
    public class MachineUsedTimeRateDto
    {
        public int SortSeq { get; internal set; }

        public double DebugRate { get; set; }

        public double FreeRate { get; set; }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public long MachinesShiftDetailId { get; set; }

        public double OfflineRate { get; set; }

        public double RunRate { get; set; }

        public string ShiftDay { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public double StopRate { get; set; }
    }
}
