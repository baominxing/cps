namespace Wimi.BtlCore.Notifications.Dto
{
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;

    [AutoMap(typeof(NotificationRecord))]
    public class GetNotificationRecordDto : FullAuditedEntityDto
    {
        public string NotificationType { get; set; }

        public string MessageType { get; set; }

        public string Status { get; set; }

        public string MessageContent { get; set; }

        public long NoticedUserId { get; set; }
    }
}
