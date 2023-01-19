using System.Collections.Generic;

namespace Wimi.BtlCore.Reasons.ReasonFeedbackCalendar.Dtos
{
    public class SelectMachineDto
    {
        public List<int> MachineIdList { get; set; }

        public int FeedbackCalendarId { get; set; }
    }
}