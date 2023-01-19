namespace Wimi.BtlCore.Notifications.Dto
{
    using System;
    using System.Collections.Generic;

    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;

    [AutoMap(typeof(NotificationRuleDetail))]
    public class NotificationRuleDetailInputDto : FullAuditedEntityDto
    {
        public int NotificationRuleId { get; set; }

        public int TriggerCondition { get; set; }

        public bool IsEnabled { get; set; }

        public string NoticeUserIds { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int ShiftSolutionId { get; set; }

        public int ShiftId { get; set; }

        public IEnumerable<int> ReferencedShiftIds { get; set; }

        public EnumTriggerType TriggerType { get; set; }
    }
}
