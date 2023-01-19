using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.WeChart
{
    /// <summary>
    /// 微信通知消息管理
    /// </summary>
    [Table("WeChatNotifications")]
    public class WeChatNotification : CreationAuditedEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeChatNotification"/> class. 
        /// 通知消息类型Id
        /// </summary>
        public WeChatNotification()
        {
        }

        public WeChatNotification(long userId, int notificationTypeId, bool isActive = true)
        {
            this.UserId = userId;
            this.NotificationTypeId = notificationTypeId;
            this.IsActive = isActive;
        }

        /// <summary>
        /// 是否启用（1启用,0未启用）
        /// </summary>
        [Comment("是否启用（1启用,0未启用）")]
        [Required]
        public bool IsActive { get; set; }

        [Comment("通知消息类型Id")]
        [Required]
        public int NotificationTypeId { get; set; }

        /// <summary>
        /// 用户key
        /// </summary>
        [Comment("用户key")]
        [Required]
        public long UserId { get; set; }
    }
}
