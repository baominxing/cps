using System;
using System.Collections.Generic;

using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Wimi.BtlCore.Feedback;

namespace Wimi.BtlCore.Reasons.ReasonFeedbacks.Dtos
{
    [AutoMap(typeof(ReasonFeedbackRecord))]
    public class ReasonFeedbackDto : AuditedEntityDto
    {
        public int MachineId { get; set; }

        public int StateId { get; set; }

        public string StateCode { get; set; }

        public string StateDisplayName { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public decimal Duration { get; set; }

        public string FeedbackPersonName { get; set; }

        public string EndUserName { get; set; }

        public string MachineName { get; set; }

        public string MachineStateDisplayName { get; set; }

        public string Hexcode { get; set; }

        public IEnumerable<string> GroupName { get; set; }

        public string ImagePath { get; set; }

        public bool Feedbacking { get; set; }
    }
}
