using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Wimi.BtlCore.Order.DefectiveReasons
{
    /// <summary>
    /// 次品原因实体
    /// </summary>
    [Table("DefectiveReasons")]
    public class DefectiveReason : FullAuditedEntity
    {
        /// <summary>
        /// 原因编号
        /// </summary>
        [Comment("不良原因编号")]
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Comment("备注")]
        [MaxLength(100)]
        public string Memo { get; set; }

        /// <summary>
        /// 原因名称
        /// </summary>
        [Comment("不良原因名称")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
