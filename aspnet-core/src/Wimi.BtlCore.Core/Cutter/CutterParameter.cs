namespace Wimi.BtlCore.Cutter
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Abp.Domain.Entities.Auditing;
    using Microsoft.EntityFrameworkCore;

    [Table("CutterParameters")]
    public class CutterParameter : FullAuditedEntity
    {
        [Required]
        [MaxLength(50)]
        [Comment("编号")]
        public string Code { get; set; }

        [Required]
        [MaxLength(50)]
        [Comment("名称")]
        public string Name { get; set; }
    }
}