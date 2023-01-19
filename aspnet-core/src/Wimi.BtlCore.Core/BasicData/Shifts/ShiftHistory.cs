using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.BasicData.Shifts
{
    [Table("ShiftHistories")]
    public class ShiftHistory : FullAuditedEntity
    {
        [Comment("班次日")]
        public DateTime ShiftDay { get; set; }

        [Comment("班次日期")]
        public int MachineShiftDetailId { get; set; }

        [Comment("设备Id")]
        public int MachineId { get; set; }

        [Comment("班次方案Id")]
        public int ShiftSolutionId { get; set; }

        [Comment("具体班次Id")]
        public int ShiftSolutionItemId { get; set; }
    }
}
