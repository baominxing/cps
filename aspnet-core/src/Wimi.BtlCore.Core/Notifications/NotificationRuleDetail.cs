namespace Wimi.BtlCore.Notifications
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Abp.Domain.Entities.Auditing;
    using Microsoft.EntityFrameworkCore;

    [Table("NotificationRuleDetails")]
    public class NotificationRuleDetail : FullAuditedEntity
    {
        [Comment("通知规则Id")]
        public int NotificationRuleId { get; set; }

        [Comment("触发条件")]
        public int TriggerCondition { get; set; }

        [Comment("是否启用")]
        public bool IsEnabled { get; set; }

        [Comment("通知人员Id(1,2,3)")]
        [MaxLength(100)]
        public string NoticeUserIds { get; set; }

        [Comment("班次方案Id")]
        public int ShiftSolutionId { get; set; }

        [Comment("班次Id")]
        public int ShiftId { get; set; }

        public IEnumerable<string> GetNoticeUserIds()
        {
            return this.NoticeUserIds.Split(',');
        }
    }
}
