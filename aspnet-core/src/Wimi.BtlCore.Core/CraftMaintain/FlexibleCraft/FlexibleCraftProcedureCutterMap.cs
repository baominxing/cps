using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Wimi.BtlCore.FmsCutters;

namespace Wimi.BtlCore.CraftMaintain
{
    /// <summary>
    /// 程序刀具映射
    /// </summary>
    [Table("FlexibleCraftProcedureCutterMaps")]
    public class FlexibleCraftProcedureCutterMap : Entity
    {
        /// <summary>
        /// 工艺Id
        /// </summary>
        [Comment("工艺Id")]
        public int CraftId { get; set; }

        /// <summary>
        /// 工艺
        /// </summary>
        [Comment("工艺")]
        public FlexibleCraft Craft { get; set; }

        /// <summary>
        /// 工序Id
        /// </summary>
        [Comment("工序Id")]
        public int CraftProcesseId { get; set; }

        /// <summary>
        /// 工序
        /// </summary>
        [Comment("工序")]
        public FlexibleCraftProcesse CraftProcesse { get; set; }

        /// <summary>
        /// 程序号
        /// </summary>
        [Comment("程序号")]
        public string ProcedureNumber { get; set; }

        /// <summary>
        /// 刀具Id
        /// </summary>
        [Comment("刀具Id")]
        public int CutterId { get; set; }

        /// <summary>
        /// 刀具
        /// </summary>
        [Comment("刀具")]
        public FmsCutter Cutter { get; set; }
    }
}
