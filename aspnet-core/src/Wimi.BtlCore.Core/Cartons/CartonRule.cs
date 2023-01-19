using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Cartons
{
    [Table("CartonRules")]
    public class CartonRule : FullAuditedEntity
    {
        [Required]
        [MaxLength(100)]
        [Comment("名称")]
        public string Name { get; set; }

        [Comment("是否启用")]
        public bool IsActive { get; set; }
    }
}
