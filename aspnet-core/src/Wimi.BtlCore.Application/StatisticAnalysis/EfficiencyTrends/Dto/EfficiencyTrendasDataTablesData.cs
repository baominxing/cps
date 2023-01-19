namespace Wimi.BtlCore.StatisticAnalysis.EfficiencyTrends.Dto
{
    using System.Collections.Generic;

    public class EfficiencyTrendasDataTablesDataDto
    {
        public EfficiencyTrendasDataTablesDataDto()
        {
            this.RateData = new Dictionary<string, string>();
        }

        public Dictionary<string, string> RateData { get; set; }
    }
}