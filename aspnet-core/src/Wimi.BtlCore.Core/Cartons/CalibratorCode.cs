using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Cartons
{
    [Table("CalibratorCodes")]
    public class CalibratorCode : Entity
    {
        /// <summary>
        /// 包装规则Id
        /// </summary>
        [Comment("包装规则Id")]
        public int CartonRuleId { get; set; }

        /// <summary>
        /// 包装规则
        /// </summary>
        [ForeignKey("CartonRuleId")]
        [Comment("包装规则")]
        public virtual CartonRule CartonRule { get; set; }

        [Comment("键")]
        public int Key { get; set; }

        [Required]
        [MaxLength(10)]
        [Comment("值")]
        public string Value { get; set; }
    }
}
