using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Order.StandardTimes
{
    /// <summary>
    /// 标准用时
    /// </summary>
    [Table("StandardTime")]
    public class StandardTime : FullAuditedEntity
    {
        /// <summary>
        /// 倍率
        /// </summary>
        [Comment("倍率")]
        public int CycleRate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Comment("备注")]
        [MaxLength(100)]
        public string Memo { get; set; }

        /// <summary>
        /// 工序Id
        /// </summary>
        [Comment("工序Id")]
        public int ProcessId { get; set; }

        /// <summary>
        /// 产品Id
        /// </summary>
        [Comment("产品Id")]
        public int ProductId { get; set; }

        /// <summary>
        /// 用时
        /// </summary>
        [Comment("用时")]
        public decimal StandardCostTime { get; set; }
    }
}
