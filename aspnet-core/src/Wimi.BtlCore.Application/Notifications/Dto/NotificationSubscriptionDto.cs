namespace Wimi.BtlCore.Notifications.Dto
{
    using System.ComponentModel.DataAnnotations;

    using Abp.Notifications;

    public class NotificationSubscriptionDto
    {
        public bool IsSubscribed { get; set; }

        [Required]
        [MaxLength(NotificationInfo.MaxNotificationNameLength)]
        public string Name { get; set; }
    }
}