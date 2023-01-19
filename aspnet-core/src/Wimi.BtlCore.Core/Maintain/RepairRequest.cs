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
    [Table("RepairRequests")]
    public class RepairRequest : FullAuditedEntity
    {
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("编号")]
        public string Code { get; set; }

        [Comment("设备Id")]
        public int MachineId { get; set; }

        [Comment("状态")]
        public EnumRepairRequestStatus Status { get; set; }

        /// <summary>
        ///  申请日期
        /// </summary>
        [Comment("申请日期")]
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        [Comment("申请人")]
        public int RequestUserId { get; set; }


        [MaxLength(BtlCoreConsts.MaxDescLength * 5)]
        [Comment("请求备注")]
        public string RequestMemo { get; set; }

        /// <summary>
        /// 维修日期
        /// </summary>
        [Comment("维修日期")]
        public DateTime? RepairDate { get; set; }

        /// <summary>
        /// 维修人
        /// </summary>
        [Comment("维修人")]
        public int? RepairUserId { get; set; }

        /// <summary>
        /// 是否关机
        /// </summary>
        [Comment("是否关机")]
        public bool IsShutdown { get; set; }

        /// <summary>
        /// 维修开始时间
        /// </summary>
        [Comment("维修开始时间")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 维修结束时间
        /// </summary>
        [Comment("维修结束时间")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 耗费时间
        /// </summary>
        [Comment("耗费时间")]
        public decimal Cost { get; set; }

        [MaxLength(BtlCoreConsts.MaxDescLength * 5)]
        [Comment("维修备注")]
        public string RepairMemo { get; set; }
    }
}
