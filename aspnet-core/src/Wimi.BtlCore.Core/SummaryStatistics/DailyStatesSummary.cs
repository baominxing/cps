using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.SummaryStatistics
{
    [Table("DailyStatesSummaries")]
    public class DailyStatesSummary : Entity<long>
    {
        /*
        [Column(TypeName = "Date")]
        public DateTime SummaryDate { get; set; }
        */
        [Comment("调试时长")] 
        public decimal DebugDuration { get; set; }

        [Comment("空闲时长")] 
        public decimal FreeDuration { get; set; }

        [Comment("最后修改时间")] 
        public DateTime LastModifyTime { get; set; }

        [Comment("设备key")] 
        public long MachineId { get; set; }

        /// <summary>
        /// 班次Id
        /// </summary>
        //[Index("IX_DailyStatesSummary", 2)]
        [Comment("班次key")] 
        public int? MachinesShiftDetailId { get; set; }

        [Comment("离线时长")] 
        public decimal OfflineDuration { get; set; }

        [Comment("运行时长")] 
        public decimal RunDuration { get; set; }

        /*
        /// <summary>
        /// 是否按照班次统计
        /// </summary>
        public bool IsByShift { get; set; }
        
             */

        /// <summary>
        /// 当天数据所属自然日
        /// </summary>
        //[Index("IX_DailyStatesSummary", 1)]
        [Comment("工程日期")] 
        public DateTime ShiftDay { get; set; }

        [Comment("停机时长")] 
        public decimal StopDuration { get; set; }

        [Comment("总共时长")] 
        public decimal TotalDuration { get; set; }
    }
}