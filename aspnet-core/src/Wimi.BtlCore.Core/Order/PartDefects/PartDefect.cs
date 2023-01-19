using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Order.PartDefects
{
    [Table("PartDefects")]
    public class PartDefect : FullAuditedEntity
    {
        [Comment("零件编号")]
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string PartNo { get; set; }

        /// <summary>
        /// 缺陷部位
        /// </summary>
        [Comment("不良部位ID")]
        [Required]
        public int DefectivePartId { get; set; }

        /// <summary>
        /// 缺陷原因
        /// </summary>
        [Comment("不良原因ID")]
        [Required]
        public int DefectiveReasonId { get; set; }

        [Comment("不良设备ID")]
        public int DefectiveMachineId { get; set; }
    }
}
