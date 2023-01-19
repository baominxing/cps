using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wimi.BtlCore.Order.Products;

namespace Wimi.BtlCore.CraftMaintain
{
    /// <summary>
    /// 工艺
    /// </summary>
    [Table("FlexibleCrafts")]
    public class FlexibleCraft : FullAuditedEntity
    {
        /// <summary>
        /// 产品Id
        /// </summary>
        [Comment("产品Id")]
        public int ProductId { get; set; }

        [Comment("产品")]
        public Product Product { get; set; }

        [MaxLength(50)]
        [Comment("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 版本--产品下的版本号唯一
        /// </summary>
        [MaxLength(50)]
        [Comment("版本--产品下的版本号唯一")]
        public string Version { get; set; }

        [NotMapped]
        [Comment("工艺工序列表")]
        public ICollection<FlexibleCraftProcesse> CraftProcesses { get; set; } = new List<FlexibleCraftProcesse>();
    }
}
