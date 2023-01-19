namespace Wimi.BtlCore.Notifications.Dto
{
    using Abp.AutoMapper;
    using Abp.Notifications;

    [AutoMapFrom(typeof(NotificationDefinition))]
    public class NotificationSubscriptionWithDisplayNameDto : NotificationSubscriptionDto
    {
        public string Description { get; set; }

        public string DisplayName { get; set; }
    }
}