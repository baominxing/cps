using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.BasicData.Shifts
{
    /// <summary>
    /// 班次方案具体信息
    /// </summary>
    [Table("ShiftSolutionItems")]
    public class ShiftSolutionItem : CreationAuditedEntity<int>
    {
        [Comment("班次持续时间 秒")]
        public decimal Duration { get; set; }

        [Comment("班次结束时间")]
        public DateTime EndTime { get; set; }

        [Comment("班次名称")]
        public string Name { get; set; }

        [ForeignKey("ShiftSolutionId")]
        [Comment("所属班次方案")]
        public ShiftSolution ShiftSolution { get; set; }

        [Comment("班次方案Id")]
        public int ShiftSolutionId { get; set; }

        [Comment("班次开始时间")]
        public DateTime StartTime { get; set; }

        [Comment("班次是否跨天")]
        public bool IsNextDay { get; set; }
    }
}
