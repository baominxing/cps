namespace Wimi.BtlCore.Reasons.ReasonFeedbackAnalysis.Dtos
{
    public class AnalysisTableData
    {
        public string SummaryDate { get; set; }

        public string StateCode { get; set; }

        public string FeedBackReason { get; set; }

        public int Times { get; set; }

        public decimal Duration { get; set; }
    }
}
