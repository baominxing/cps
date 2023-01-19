using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Order.WorkOrders
{
    /// <summary>
    /// 工单次品记录
    /// </summary>
    [Table("WorkOrderDefectiveRecords")]
    public class WorkOrderDefectiveRecords : CreationAuditedEntity
    {
        /// <summary>
        /// 次品数
        /// </summary>
        [Comment("数量")]
        [Required]
        public int Count { get; set; }

        /// <summary>
        /// 次品原因Id
        /// </summary>
        [Comment("不良原因key")]
        [Required]
        public int DefectiveReasonsId { get; set; }

        [Comment("工单任务key")]
        [Required]
        public int WorkOrderTaskId { get; set; }
    }
}
