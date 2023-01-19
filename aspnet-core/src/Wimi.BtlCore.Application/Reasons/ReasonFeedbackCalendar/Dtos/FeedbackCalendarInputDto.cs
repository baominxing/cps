using Abp.AutoMapper;
using Abp.Runtime.Validation;
using System;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Feedback;

namespace Wimi.BtlCore.Reasons.ReasonFeedbackCalendar.Dtos
{
    [AutoMap(typeof(FeedbackCalendar))]
     public class FeedbackCalendarInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Cron { get; set; }

        public string StateCode { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int Duration { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public long? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public long? CreatorUserId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "Id";
            }
        }
    }
}
