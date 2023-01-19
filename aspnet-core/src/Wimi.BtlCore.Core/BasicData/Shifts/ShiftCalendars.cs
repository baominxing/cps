using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Wimi.BtlCore.BasicData.Shifts
{
    /// <summary>
    /// 班次日历表
    /// </summary>
    [Table("ShiftCalendars")]
    public class ShiftCalendar : AuditedEntity<long>
    {
        /// <summary>
        /// 班次方案Id
        /// </summary>
        [Comment("班次方案Id")]
        public int ShiftSolutionId { get; set; }

        /// <summary>
        /// 班次排班Id
        /// </summary>
        [Comment("具体班次Id")]
        public int ShiftItemId { get; set; }

        /// <summary>
        /// 班次顺序
        /// </summary>
        [Comment("具体班次顺序")]
        public int ShiftItemSeq { get; set; }

        /// <summary>
        /// 设备具体班次Id
        /// </summary>
        [Comment("设备班次Id")]
        public int MachineShiftDetailId { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        [Comment("设备Id")]
        public int MachineId { get; set; }

        /// <summary>
        /// 班次开始时间
        /// </summary>
        [Comment("班次开始时间")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 班次结束时间
        /// </summary>
        [Comment("班次结束时间")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 班次时长，秒
        /// </summary>
        [Comment("班次时长，秒")]
        public long Duration { get; set; }

        [Comment("班次日")]
        public DateTime ShiftDay { get; set; }

        /// <summary>
        /// 2020-02-08 日
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        [Comment("班次日名称 2020-02-08 日")]
        public string ShiftDayName { get; set; }

        [Comment("班次周")]
        public int ShiftWeek { get; set; }

        /// <summary>
        /// 2020-03 周
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        [Comment("班次周名称 2020-03 周")]
        public string ShiftWeekName { get; set; }

        [Comment("班次月")]
        public int ShiftMonth { get; set; }

        /// <summary>
        /// 2020-01 月
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        [Comment("班次月名称 2020-01 月")]
        public string ShiftMonthName { get; set; }

        [Comment("班次年")]
        public int ShiftYear { get; set; }

        /// <summary>
        /// 2020 年
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        [Comment("班次年名称")]
        public string ShiftYearName { get; set; }
    }
}
