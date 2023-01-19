namespace Wimi.BtlCore.Notifications.Dto
{
    using Abp.Notifications;

    using Wimi.BtlCore.Dto;

    public class GetUserNotificationsInputDto : DatatablesPagedInputDto
    {
        public UserNotificationState? State { get; set; }
    }
}