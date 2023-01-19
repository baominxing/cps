using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Maintain
{
    /// <summary>
    ///     设备保养计划
    /// </summary>
    [Table("MaintainPlans")]
    public class MaintainPlan : FullAuditedEntity
    {
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("编号")]
        public string Code { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("名称")]
        public string Name { get; set; }

        [Comment("设备编号")]
        public int MachineId { get; set; }

        [Comment("状态")]
        public EnumMaintainPlanStatus Status { get; set; }

        /// <summary>
        /// 计划生效日期
        /// </summary>
        [Comment("计划生效日期")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 计划失效日期
        /// </summary>
        [Comment("计划失效日期")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 间隔日期
        /// </summary>
        [Comment("间隔日期")]
        public int IntervalDate { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        [Comment("负责人")]
        public int PersonInChargeId { get; set; }

        [MaxLength(BtlCoreConsts.MaxDescLength * 5)]
        [Comment("备注")]
        public string Memo { get; set; }
    }
}
