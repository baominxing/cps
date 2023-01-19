using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Feedback
{
    [Table("FeedbackCalendars")]
    public class FeedbackCalendar:FullAuditedEntity
    {
        [Comment("编号")]
        public string Code { get; set; }

        [Comment("名称")]
        public string Name { get; set; }

        [Comment("表达式")]
        public string Cron { get; set; }

        [Comment("状态编号")]
        public string StateCode { get; set; }

        // 分钟
        [Comment("持续时间 分钟")]
        public int Duration { get; set; }

    }
}