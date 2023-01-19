namespace Wimi.BtlCore.Notifications.Dto
{
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;

    [AutoMap(typeof(NotificationRule))]
    public class GetNotificationRuleDto : FullAuditedEntityDto
    {
        public string DeviceGroupIds { get; set; }

        public string Name { get; set; }

        public int MemberCount { get; set; }

        public string DeviceGroupNames { get; set; }

        public EnumMessageType MessageType { get; set; }

        public EnumTriggerType TriggerType { get; set; }
    }
}
