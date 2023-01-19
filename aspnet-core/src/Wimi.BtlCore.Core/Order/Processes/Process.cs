using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Order.Processes
{
    /// <summary>
    /// 工序
    /// </summary>
    [Table("Process")]
    public class Process : FullAuditedEntity
    {
        /// <summary>
        /// 工序编号
        /// </summary>
        [Comment("编码")]
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Comment("描述")]
        [MaxLength(100)]
        public string Memo { get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        [Comment("名称")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
