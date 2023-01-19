using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.BasicData.Shifts
{
    /// <summary>
    /// 生效班次明细表
    /// </summary>
    [Table("MachinesShiftDetails")]
    public class MachinesShiftDetail : CreationAuditedEntity<int>
    {
        [Comment("设备Id")]
        public int MachineId { get; set; }

        //[Index]
        [Comment("班次日")]
        public DateTime ShiftDay { get; set; }

        [Comment("班次方案Id")]
        public int ShiftSolutionId { get; set; }

        [Comment("具体班次Id")]
        public int ShiftSolutionItemId { get; set; }
    }


}
