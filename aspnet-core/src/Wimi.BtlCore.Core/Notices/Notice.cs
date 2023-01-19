using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Notices
{
    [Table("Notices")]
    public class Notice : AuditedEntity
    {
        [Comment("公告内容")]
        [Required]
        [MaxLength(BtlCoreConsts.MaxDescLength * 5)]
        public string Content { get; set; }

        [Comment("是否启用")]
        [Required]
        public bool IsActive { get; set; }

        [Comment("车间代码")]
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string RootDeviceGroupCode { get; set; }
    }
}
