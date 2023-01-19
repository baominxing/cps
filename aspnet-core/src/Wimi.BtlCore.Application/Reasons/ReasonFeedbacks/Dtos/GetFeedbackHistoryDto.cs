using Abp.Runtime.Validation;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Reasons.ReasonFeedbacks.Dtos
{
    public class GetFeedbackHistoryDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int Id { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrWhiteSpace(this.Sorting))
            {
                this.Sorting = "StartTime";
            }
        }
    }
}
