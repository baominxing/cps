namespace Wimi.BtlCore.Notifications.Dto
{
    using System.Collections.Generic;

    using Abp.Application.Services.Dto;
    using Abp.Notifications;

    public class GetNotificationsOutputDto : PagedResultDto<UserNotification>
    {
        public GetNotificationsOutputDto(int totalCount, int unreadCount, List<UserNotification> notifications)
            : base(totalCount, notifications)
        {
            this.UnreadCount = unreadCount;
        }

        public int UnreadCount { get; set; }
    }
}