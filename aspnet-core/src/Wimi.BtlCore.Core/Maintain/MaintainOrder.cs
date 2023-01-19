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
    [Table("MaintainOrders")]
    public class MaintainOrder : FullAuditedEntity
    {
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("保养计划编号")]
        public string MaintainPlanCode { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("编号")]
        public string Code { get; set; }

        [Comment("状态")]
        public EnumMaintainOrderStatus Status { get; set; }

        [Comment("设备Id")]
        public int MachineId { get; set; }

        /// <summary>
        /// 计划保养日期
        /// </summary>
        [Comment("计划保养日期")]
        public DateTime ScheduledDate { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Comment("开始时间")]
        public DateTime? StartTime { get; set; }


        /// <summary>
        /// 实际保养日期
        /// </summary>
        [Comment("实际保养日期")]
        public DateTime? MaintainDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Comment("结束时间")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 耗费时间
        /// </summary>
        [Comment("耗费时间")]
        public decimal Cost { get; set; }

        /// <summary>
        /// 保养人
        /// </summary>
        [Comment("保养用户Id")]
        public int MaintainUserId { get; set; }

        [MaxLength(BtlCoreConsts.MaxDescLength * 5)]
        [Comment("备注")]
        public string Memo { get; set; }
    }
}
