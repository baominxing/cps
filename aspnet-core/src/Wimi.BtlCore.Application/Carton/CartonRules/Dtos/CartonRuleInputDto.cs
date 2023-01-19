using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using Wimi.BtlCore.Cartons;

namespace Wimi.BtlCore.Carton.CartonRules.Dtos
{
    [AutoMap(typeof(CartonRule))]
    public class CartonRuleInputDto
    {
        public int? Id { get; set; }

        /// <summary>
        /// 规则名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
    }
}
