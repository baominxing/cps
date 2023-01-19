namespace Wimi.BtlCore.Notifications.Dto
{
    using System.Collections.Generic;

    public class UpdateNotificationSettingsInputDto
    {
        public List<NotificationSubscriptionDto> Notifications { get; set; }

        public bool ReceiveNotifications { get; set; }
    }
}