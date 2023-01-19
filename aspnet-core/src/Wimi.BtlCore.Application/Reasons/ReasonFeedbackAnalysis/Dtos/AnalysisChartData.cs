using System;

namespace Wimi.BtlCore.Reasons.ReasonFeedbackAnalysis.Dtos
{
    public class AnalysisChartData
    {
        public string MachineName { get; set; }

        public DateTime SummaryDate { get; set; }

        public int Times { get; set; }

        public decimal Duration { get; set; }

        public int HorizontalValue { get; set; }

        public int VerticalValue { get; set; }
    }
}
