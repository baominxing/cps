using Abp.AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wimi.BtlCore.Cartons;

namespace Wimi.BtlCore.Carton.CartonRules.Dtos
{
    public class RuleDetailDto
    {
        public AdditionalInfos AdditionalInfo { get; set; }

        public List<RuleDetailItem> RuleDetailItems { get; set; }
    }

    [AutoMap(typeof(CartonRuleDetail))]
    public class RuleDetailItem
    {
        public int Id { get; set; }

        public int CartonRuleId { get; set; }

        public int SequenceNo { get; set; }

        public EnumRuleType Type { get; set; }

        public int Length { get; set; }

        public int ExpansionKey { get; set; }

        [Required]
        [MaxLength(100)]
        public string Value { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public string Remark { get; set; }

        public int Merge { get; set; }
    }

    /// <summary>
    /// 额外信息
    /// </summary>
    public class AdditionalInfos
    {
        /// <summary>
        /// 箱码长度
        /// </summary>
        public int CartonNoLenth { get; set; }

        /// <summary>
        /// 预览箱码
        /// </summary>
        public string PreviewCartonNo { get; set; }
    }
}
