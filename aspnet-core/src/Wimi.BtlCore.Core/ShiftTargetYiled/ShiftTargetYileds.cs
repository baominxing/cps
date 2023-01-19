namespace Wimi.BtlCore.ShiftTargetYiled
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Abp.Domain.Entities.Auditing;
    using Microsoft.EntityFrameworkCore;
    using Wimi.BtlCore.BasicData.Shifts;
    using Wimi.BtlCore.Order.Products;

    [Table("ShiftTargetYileds")]
    public class ShiftTargetYileds : FullAuditedEntity
    {
        [Comment("产品key")]
        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Products { get; set; }

        [Comment("班次日")]
        [Required]
        public DateTime ShiftDay { get; set; }

        [Comment("班次key")]
        [Required]
        public int ShiftSolutionItemId { get; set; }

        [ForeignKey("ShiftSolutionItemId")]
        public virtual ShiftSolutionItem ShiftSolutionItem { get; set; }

        [Comment("目标产量")]
        [Required]
        public int TargetYiled { get; set; }

        public string GetShiftDayString()
        {
            return this.ShiftDay.ToString("yyyy-MM-dd");
        }
    }
}