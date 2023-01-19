namespace Wimi.BtlCore.Notifications.Dto
{
    using System.Collections.Generic;

    public class GetNotificationSettingsOutputDto
    {
        public List<NotificationSubscriptionWithDisplayNameDto> Notifications { get; set; }

        public bool ReceiveNotifications { get; set; }
    }
}