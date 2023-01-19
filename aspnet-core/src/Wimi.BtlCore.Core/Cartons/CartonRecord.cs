using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.BasicData.Shifts;

namespace Wimi.BtlCore.Cartons
{
    [Table("CartonRecords")]
    public class CartonRecord : FullAuditedEntity
    {
        [Required]
        [MaxLength(100)]
        [Comment("箱号")]
        public string CartonNo { get; set; }

        [Comment("箱Id")]
        public int CartonId { get; set; }

        [Required]
        [Comment("工件二维码")]
        public string PartNo { get; set; }

        [Required]
        [Comment("班次日")]
        public DateTime ShiftDay { get; set; }

        [Comment("具体班次Id")]
        public int ShiftSolutionItemId { get; set; }

        [ForeignKey("ShiftSolutionItemId")]
        [Comment("具体班次")]
        public virtual ShiftSolutionItem ShiftSolutionItems { get; set; }

    }
}
