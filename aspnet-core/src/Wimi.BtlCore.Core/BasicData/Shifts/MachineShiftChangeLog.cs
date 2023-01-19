using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.BasicData.Shifts
{
    [Table("MachineShiftChangeLogs")]
    public class MachineShiftChangeLog : CreationAuditedEntity<int>
    {
        [Comment("设备Id")]
        public int MachineId { get; set; }

        [Comment("显示顺序")]
        public int Seq { get; set; }

        [Comment("班次方案Id")]
        public int ShiftSolutionId { get; set; }

        [Comment("操作类型")]
        public OperationType Type { get; set; }
    }
}
