using System.Collections.Generic;

namespace Wimi.BtlCore.Reasons.ReasonFeedbackAnalysis.Dtos
{
    public class ReasonFeedBackAnalysisDto
    {
        public List<AnalysisChartData> ChartData { get; set; }

        public List<AnalysisTableData> TableData { get; set; }
    }
}
