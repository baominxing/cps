namespace Wimi.BtlCore.StatisticAnalysis.StatusDistributionMap.Dto
{
    public class DailyStatusSummaryDto
    {
        public string StateCode { get; set; }

        public string StateName { get; set; }

        public double Hour { get; set; }

        public double Percent { get; set; }

        public string Hexcode { get; set; }
    }
}