namespace Wimi.BtlCore.Notifications.Dto
{
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;

    [AutoMap(typeof(NotificationRule))]
    public class NotificationRuleInputDto : FullAuditedEntityDto
    {
        public string DeviceGroupIds { get; set; }

        public string Name { get; set; }

        public EnumMessageType MessageType { get; set; }

        public EnumTriggerType TriggerType { get; set; }
    }
}
