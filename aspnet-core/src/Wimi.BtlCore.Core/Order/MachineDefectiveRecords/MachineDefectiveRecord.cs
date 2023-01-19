using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Order.DefectiveReasons;

namespace Wimi.BtlCore.Order.MachineDefectiveRecords
{

    [Table("MachineDefectiveRecords")]
    public class MachineDefectiveRecord : FullAuditedEntity
    {
        [Comment("数量")]
        [Required]
        public int Count { get; set; }

        [Comment("不良原因ID")]
        [Required]
        public int DefectiveReasonsId { get; set; }

        [ForeignKey("DefectiveReasonsId")]
        public virtual DefectiveReason DefectiveReason { get; set; }

        [Comment("设备ID")]
        public int MachineId { get; set; }

        [Comment("班次信息ID")]
        public int ShiftSolutionItemId { get; set; }

        [ForeignKey("ShiftSolutionItemId")]
        public virtual ShiftSolutionItem ShiftSolutionItems { get; set; }

        [Comment("日期")]
        public DateTime ShiftDay { get; set; }

        [Comment("产品ID")]
        public int ProductId { get; set; }
    }
}
