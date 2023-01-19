
using Wimi.BtlCore.Reasons.ReasonFeedbackCalendar.Dtos;

namespace Wimi.BtlCore.Web.Models.Reasons.ReasonFeedback
{
    public class FeedbackCalendarViewModel
    {
        public FeedbackCalendarDto Dto { get; set; } = new FeedbackCalendarDto();

        public bool IsEditMode { get; set; }
    }
}