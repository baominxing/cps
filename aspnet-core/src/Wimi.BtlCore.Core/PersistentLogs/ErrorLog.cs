using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.PersistentLogs
{
    [Table("ErrorLogs")]
    public class ErrorLog : Entity<long>
    {
        [Comment("传入参数")]
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string InParams { get; set; }

        [Comment("返回特定于引用它的 CATCH 块作用域的错误行号")]
        public int Line { get; set; }

        [Comment("返回特定于它被引用 CATCH 块作用域的错误消息")]
        [MaxLength(BtlCoreConsts.MaxDescLength * 20)]
        public string Message { get; set; }

        [Comment("错误码")]
        public int? Number { get; set; }

        [Comment("错误发生时间")]
        public DateTime OccurDate { get; set; }

        [Comment("返回存储过程或触发器的名称")]
        [MaxLength(BtlCoreConsts.MaxDescLength * 2)]
        public string ProcName { get; set; }

        [Comment("返回特定于引用它的 CATCH 块的范围的错误严重级别")]
        public int? Serverity { get; set; }

        [Comment("返回引用它的 CATCH 块范围特有的错误状态")]
        public int? State { get; set; }
    }
}
