using System;

namespace Wimi.BtlCore.Reasons.ReasonFeedbackAnalysis.Dtos
{
    public class ReasonFeedBackAnalysisRequestDto
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int DeviceGroupId { get; set; }
    }
}
