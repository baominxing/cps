using Abp.Runtime.Validation;
using System;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Reasons.ReasonFeedbackAnalysis.Dtos
{
    public class ReasonFeedBackAnalysisDetailRequestDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime SummaryDate { get; set; }

        public string StateCode { get; set; }

        public int DeviceGroupId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "SummaryDate desc";
            }
        }
    }
}
