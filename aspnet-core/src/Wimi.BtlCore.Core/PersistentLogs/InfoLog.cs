using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.PersistentLogs
{
    [Table("InfoLogs")]
    public class InfoLog
    {
        [Comment("执行时长，秒")]
        public int? Duration { get; set; }

        [Comment("结束时间")]
        public DateTime EndTime { get; set; }

        [Comment("GUID")]
        public Guid Id { get; set; }

        [Comment("执行存储过程名称")]
        [MaxLength(BtlCoreConsts.MaxDescLength * 2)]
        public string ProcName { get; set; }

        [Comment("开始时间")]
        public DateTime StartTime { get; set; }
    }
}
