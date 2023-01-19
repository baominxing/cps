namespace Wimi.BtlCore.StatisticAnalysis.States.Dto
{
    using System.Collections.Generic;

    public class StatesStatisticForChartDto
    {
        public StatesStatisticForChartDto()
        {
            this.StopRate = new List<decimal>();
            this.RunRate = new List<decimal>();
            this.FreeRate = new List<decimal>();
            this.OfflineRate = new List<decimal>();
            this.DebugRate = new List<decimal>();
            this.Yield = new List<decimal>();
            this.SummaryDate = new List<string>();
        }

        public List<decimal> DebugRate { get; set; }

        public List<decimal> FreeRate { get; set; }

        public List<decimal> OfflineRate { get; set; }

        public List<decimal> RunRate { get; set; }

        public List<decimal> StopRate { get; set; }

        public List<string> SummaryDate { get; set; }

        public List<decimal> Yield { get; set; }
    }
}