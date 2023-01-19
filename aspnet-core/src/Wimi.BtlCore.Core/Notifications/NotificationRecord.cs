namespace Wimi.BtlCore.Notifications
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Abp.Domain.Entities.Auditing;
    using Microsoft.EntityFrameworkCore;

    [Table("NotificationRecords")]
    public class NotificationRecord : FullAuditedEntity
    {
        [Comment("通知方式（微信，邮件）")]
        public EnumNotificationType NotificationType { get; set; }

        [Comment("消息类型")]
        public EnumMessageType MessageType { get; set; }

        [Comment("消息状态(0:未发送,1:已发送)")]
        public EnumNotificationRecordStatus Status { get; set; }

        [Comment("消息内容")]
        [MaxLength(2000)]
        public string MessageContent { get; set; }

        [Comment("通知人Id")]
        public long NoticedUserId { get; set; }
    }
}
