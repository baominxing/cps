using System;

namespace WIMI.BTL.ReasonFeedback.Dto
{
    public class CheckTimeResultDto
    {
        public CheckTimeResultDto()
        {
    
        }

        public CheckTimeResultDto(string result)
        {
            Result = result;
        }

        public string Result { get; set; }

        public string ErrprTimeRange { get; set; }

        public ErrorReasonDto ErrorReason { get; set; }
    }

    public class ErrorReasonDto
    {
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
