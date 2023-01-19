using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.PersistentLogs
{
    [Table("InfoLogDetails")]
    public class InfoLogDetail : Entity<long>
    {
        [Comment("影响的行数")]
        public int? AffectedRowCount { get; set; }

        [Comment("执行时长，秒")]
        public int Duration { get; set; }

        [Comment("结束时间")]
        public DateTime EndTime { get; set; }

        [Comment("InfoLogs")]
        public Guid InfoLogId { get; set; }

        [Comment("信息")]
        public string Message { get; set; }

        [Comment("步骤")]
        public string Step { get; set; }

        [Comment("开始时间")]
        public DateTime StratTime { get; set; }
    }
}
