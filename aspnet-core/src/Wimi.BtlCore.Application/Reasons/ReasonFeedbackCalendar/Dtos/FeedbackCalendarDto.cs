using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using Wimi.BtlCore.Feedback;

namespace Wimi.BtlCore.Reasons.ReasonFeedbackCalendar.Dtos
{
    [AutoMap(typeof(FeedbackCalendar))]
    public class FeedbackCalendarDto: FullAuditedEntityDto<int>
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Cron { get; set; }

        public string StateCode { get; set; }

        public string StateName { get; set; }

        public int Duration { get; set; }

        public string Error { get; set; }

        public DateTime? LastExecution { get; set; }

        public DateTime? NextExecution { get; set; }

        public string LastJobState { get; set; }
    }
}

