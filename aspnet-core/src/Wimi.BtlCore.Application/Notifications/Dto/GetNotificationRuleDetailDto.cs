namespace Wimi.BtlCore.Notifications.Dto
{
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;

    [AutoMap(typeof(NotificationRuleDetail))]
    public class GetNotificationRuleDetailDto : FullAuditedEntityDto
    {
        public int Order { get; set; }

        public int NotificationRuleId { get; set; }

        public int TriggerCondition { get; set; }

        public int ShiftSolutionId { get; set; }

        public int ShiftId { get; set; }

        public string ShiftInfoName { get; set; }

        public bool IsEnabled { get; set; }

        public string NoticeUserIds { get; set; }

        public string NoticeUserNames { get; set; }
    }
}
