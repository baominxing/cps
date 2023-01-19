namespace Wimi.BtlCore.RealtimeIndicators.Parameters.Dto
{
    public class ParamComparisonInputDto
    {
        public int MachineId { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string[] ParamCodes { get; set; }
    }
}