using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Plan
{
    [Table("PlanTargets")]
    public class PlanTarget : Entity
    {
        [Comment("班次方案ID")]
        public int SolutionId { get; set; }

        [Comment("班次ID")]
        public int ShiftId { get; set; }

        [Comment("班次名称")]
        [MaxLength(50)]
        public string ShiftName { get; set; }

        [Comment("班次目标量")]
        public int ShiftTargetAmount { get; set; }

    }
}