namespace Wimi.BtlCore.Cutter
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Abp.Domain.Entities.Auditing;
    using Microsoft.EntityFrameworkCore;

    [Table("CutterTypes")]
    public class CutterType : FullAuditedEntity
    {
        [Required]
        [MaxLength(50)]
        [Comment("编号")]
        public string Code { get; set; }

        [Required]
        [MaxLength(50)]
        [Comment("刀具编号前缀")]
        public string CutterNoPrefix { get; set; }

        [Comment("刀具编号前缀是否能编辑")]
        public bool IsCutterNoPrefixCanEdit { get; set; }

        [Required]
        [MaxLength(50)]
        [Comment("名称")]
        public string Name { get; set; }

        [Comment("父节点Id")]
        public int? PId { get; set; }
    }
}