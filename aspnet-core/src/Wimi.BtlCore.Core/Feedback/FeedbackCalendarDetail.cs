using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Feedback
{
    [Table("FeedbackCalendarDetails")]
    public class FeedbackCalendarDetail: FullAuditedEntity
    {
        [Comment("设备Id")]
        public int MachineId { get; set; }

        [Comment("反馈频率Id")]
        public int FeedbackCalendarId { get; set; }
    }
}