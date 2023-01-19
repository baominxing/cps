using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.WeChart
{
    /// <summary>
    /// 通知消息类型
    /// </summary>
    [Table("NotificationTypes")]
    public class NotificationType : CreationAuditedEntity
    {
        public NotificationType()
        {
        }

        public NotificationType(string displayName, int? parentId = null)
        {
            this.DisplayName = displayName;
            this.ParentId = parentId;
        }

        /// <summary>
        /// 用户key
        /// </summary>
        [Comment("用户key")]
        [Required]
        [StringLength(BtlCoreConsts.MaxDescLength)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Code，参照部门实现方式
        /// </summary>
        [Comment("Code，参照部门实现方式")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 父节点Id
        /// </summary>
        [Comment("父节点Id")]
        public int? ParentId { get; set; }
    }
}
