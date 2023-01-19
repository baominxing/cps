
namespace Wimi.BtlCore.Parameters.Dto
{
    public class HistoryParameterListRequestDto
    {
         public int? MachineId { get; set; }

        public string ObjectId { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public int Start { get; set; }

        public int? Length { get; set; }

        public bool PageDown { get; set; }

    }
}
