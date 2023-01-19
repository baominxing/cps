using Abp.AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wimi.BtlCore.Cartons;

namespace Wimi.BtlCore.Carton.CartonRules.Dtos
{
    public class RuleDetailInputDto
    {
        [Required]
        public List<RuleDetailInputItem> RuleDetailInputItems { get; set; }
    }

    [AutoMap(typeof(CartonRuleDetail))]
    public class RuleDetailInputItem
    {
        public int? Id { get; set; }

        public virtual CartonRule CartonRule { get; set; }

        public int CartonRuleId { get; set; }

        public int SequenceNo { get; set; }

        [Required]
        public EnumRuleType Type { get; set; }

        public int Length { get; set; }

        public int ExpansionKey { get; set; }

        [MaxLength(100)]
        public string Value { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public string Remark { get; set; }
    }
}
