using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Order.WorkOrders
{
    [Table("WorkOrderTasks")]
    public class WorkOrderTasks : AuditedEntity
    {
        /// <summary>
        /// 次品数
        /// </summary>
        [Comment("不合格数量")]
        [Required]
        public int DefectiveCount { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Comment("结束时间")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        [Comment("设备key")]
        [Required]
        public int MachineId { get; set; }

        /// <summary>
        /// 产出量= 正品数+次品数
        /// </summary>
        [Comment("输出数量")]
        [Required]
        public int OutputCount { get; set; }

        /// <summary>
        /// 合格数量/正品数
        /// </summary>
        [Comment("合格数量")]
        [Required]
        public int QualifiedCount { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Comment("开始时间")]
        [Required]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 人员Id
        /// </summary>
        [Comment("用户key")]
        [Required]
        public long UserId { get; set; }

        /// <summary>
        /// 工单次品数记录
        /// </summary>
        [ForeignKey("WorkOrderTaskId")]
        public virtual ICollection<WorkOrderDefectiveRecords> WorkOrderDefectiveRecordses { get; set; }

        /// <summary>
        /// 工单Id
        /// </summary>
        [Comment("工单key")]
        [Required]
        public int WorkOrderId { get; set; }
    }
}
