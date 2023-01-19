using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Order.Crafts
{
    /// <summary>
    /// 产品组合工序
    /// </summary>
    [Table("CraftProcesses")]
    public class CraftProcess : FullAuditedEntity
    {
        [ForeignKey("CraftId")]
        public virtual Craft Craft { get; set; }

        /// <summary>
        /// 产品工艺Id
        /// </summary>
        [Comment("产品工艺Id")]
        public int CraftId { get; set; }

        /// <summary>
        /// 是否是组后工序
        /// </summary>
        [Comment("是否是最后工序")]
        public bool IsLastProcess { get; set; }

        /// <summary>
        /// 工艺代码
        /// </summary>
        [Comment("工艺代码")]
        [MaxLength(50)]
        public string ProcessCode { get; set; }

        /// <summary>
        /// 工艺ID
        /// </summary>
        [Comment("工艺ID")]
        public int ProcessId { get; set; }

        /// <summary>
        /// 工艺名称
        /// </summary>
        [Comment("工艺名称")]
        [MaxLength(50)]
        public string ProcessName { get; set; }

        /// <summary>
        /// 工艺加工顺序
        /// </summary>
        [Comment("工艺加工顺序")]
        public int ProcessOrder { get; set; }
    }
}
