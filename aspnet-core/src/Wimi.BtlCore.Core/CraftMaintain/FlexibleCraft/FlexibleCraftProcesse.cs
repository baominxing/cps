using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.CraftMaintain
{
    /// <summary>
    /// 工序
    /// </summary>
    [Table("FlexibleCraftProcesses")]
    public class FlexibleCraftProcesse : FullAuditedEntity
    {
        [MaxLength(50)]
        [Comment("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Comment("顺序号")]
        public int Sequence { get; set; }

        /// <summary>
        /// 夹具Id
        /// </summary>
        [Comment("夹具Id")]
        public int TongId { get; set; }

        [Comment("夹具")]
        public Tong Tong { get; set; }
    }
}
