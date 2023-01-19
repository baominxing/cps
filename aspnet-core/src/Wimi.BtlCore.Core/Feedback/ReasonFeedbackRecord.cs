using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Wimi.BtlCore.Feedback
{
    /// <inheritdoc />
    /// <summary>
    ///     反馈原因记录实体
    /// </summary>
    [Table("ReasonFeedbackRecords")]
    public class ReasonFeedbackRecord : FullAuditedEntity
    {
        [Required]
        [Comment("设备Id")]
        public int MachineId { get; set; }

        [Required]
        [Comment("状态Id")]
        public int StateId { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength * 2)]
        [Comment("状态编号")]
        public string StateCode { get; set; }

        [Required]
        [Comment("开始时间")]
        public DateTime StartTime { get; set; }

        [Comment("结束时间")]
        public DateTime? EndTime { get; set; }

        [Comment("结束用户Id")]
        public int? EndUserId { get; set; }

        [Comment("持续时间")]
        public decimal Duration { get; set; }
    }
}
