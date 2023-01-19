using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.CraftMaintain
{
    /// <summary>
    /// 工艺工序映射
    /// </summary>
    [Table("FlexibleCraftProcesseMaps")]
    public class FlexibleCraftProcesseMap : Entity
    {
        /// <summary>
        /// 工艺Id
        /// </summary>
        [Comment("工艺Id")]
        public int CraftId { get; set; }

        [Comment("工艺")]
        public FlexibleCraft Craft { get; set; }

        /// <summary>
        /// 工序Id
        /// </summary>
        [Comment("工序Id")]
        public int CraftProcesseId { get; set; }

        [Comment("工序")]
        public FlexibleCraftProcesse CraftProcesse { get; set; }
    }
}
