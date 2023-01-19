using Abp.Runtime.Validation;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Reasons.ReasonFeedbackCalendar.Dtos
{
    public class SelectMachinesInputDto: PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int FeedbackCalendarId { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "CreationTime";
            }
        }
    }
}