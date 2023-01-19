using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Cartons
{
    [Table("CartonRuleDetails")]
    public class CartonRuleDetail : FullAuditedEntity
    {
        [Comment("箱号规则Id")]
        public int CartonRuleId { get; set; }

        [ForeignKey("CartonRuleId")]
        [Comment("箱号规则")]
        public virtual CartonRule CartonRule { get; set; }

        [Comment("顺序号")]
        public int SequenceNo { get; set; }

        [Comment("类型")]
        public EnumRuleType Type { get; set; }

        [Comment("长度")]
        public int Length { get; set; }

        [Comment("扩展键")]
        public int ExpansionKey { get; set; }

        [MaxLength(100)]
        [Comment("值")]
        public string Value { get; set; }

        [Comment("开始位")]
        public int StartIndex { get; set; }

        [Comment("结束位")]
        public int EndIndex { get; set; }

        [Comment("备注")]
        public string Remark { get; set; }
    }
}
