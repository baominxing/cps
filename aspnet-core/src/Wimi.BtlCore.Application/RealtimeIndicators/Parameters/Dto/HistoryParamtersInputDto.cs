using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.RealtimeIndicators.Parameters.Dto
{

    public class HistoryParamtersInputDto : PagedSortedAndFilteredInputDto
    {
        public string EndTime { get; set; }

        public string MachineCode { get; set; }

        public int? MachineId { get; set; }

        public string ObjectId { get; set; }

        public bool PageDown { get; set; }

        public string StartTime { get; set; }
    }
}