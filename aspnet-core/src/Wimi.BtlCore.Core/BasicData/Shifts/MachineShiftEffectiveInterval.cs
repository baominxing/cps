using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.Timing.Utils;

namespace Wimi.BtlCore.BasicData.Shifts
{
    /// <summary>
    /// 设备班次方案
    /// </summary>
    [Table("MachineShiftEffectiveIntervals")]
    public class MachineShiftEffectiveInterval : FullAuditedEntity
    {
        [Comment("结束时间")]
        public DateTime EndTime { get; set; }

        [ForeignKey("MachineId")]
        [Comment("设备")]
        public Machine Machine { get; set; }

        [Comment("设备Id")]
        public int MachineId { get; set; }

        [ForeignKey("ShiftSolutionId")]
        [Comment("班次方案")]
        public ShiftSolution ShiftSolution { get; set; }

        [Comment("班次方案Id")]
        public int ShiftSolutionId { get; set; }

        [Comment("开始时间")]
        public DateTime StartTime { get; set; }


        public MachineShiftEffectiveInterval Clone(int shiftSolutionId)
        {
            return new MachineShiftEffectiveInterval()
            {
                MachineId = this.MachineId,
                ShiftSolutionId = shiftSolutionId,
                EndTime = this.EndTime,
                StartTime = this.StartTime.AddDays(1)
            };
        }

        public bool IsSanmeDay()
        {
            return this.StartTime == this.EndTime && this.StartTime == DateTime.Now.ConvertTodayToCstTime();
        }
    }
}
