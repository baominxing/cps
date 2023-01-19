using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore.Reasons.ReasonFeedbackAnalysis.Dtos
{
    public class ReasonFeedBackAnalysisDetailDto
    {
        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public string FeedBackReason { get; set; }

        public int Times { get; set; }

        public decimal Duration { get; set; }
    }
}
