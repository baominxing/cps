using System.Collections.Generic;

namespace Wimi.BtlCore.Parameters.Dto
{
    public class GetParamtersListDto
    {
        public GetParamtersListDto()
        {
            this.LineChartParamtersList = new List<ParamsItem>();
            this.BlockChartParamtersList = new List<ParamsItem>();
            this.GaugeParamtersList = new List<ParamsItem>();
        }

        public IEnumerable<ParamsItem> BlockChartParamtersList { get; set; }

        public string CreationTime { get; set; }

        public IEnumerable<ParamsItem> GaugeParamtersList { get; set; }

        public IEnumerable<ParamsItem> LineChartParamtersList { get; set; }
    }
}
