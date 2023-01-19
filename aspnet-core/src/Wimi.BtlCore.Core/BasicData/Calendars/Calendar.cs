using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.BasicData.Calendars
{
    /// <summary>
    /// 日历表
    /// </summary>
    [Table("Calendars")]
    // ReSharper disable All
    public class Calendar
    {
        /// <summary>
        /// 自然日
        /// </summary>
        [Comment("自然日")]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// 自然日数字格式
        /// </summary>
        [Comment("自然日数字格式")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DateKey { get; set; }

        /// <summary>
        /// 当前月第几天
        /// </summary>
        [Comment("当前月第几天")]
        public byte Day { get; set; }

        /// <summary>
        /// 当前年第几天
        /// </summary>
        [Comment("当前年第几天")]
        public short DayOfYear { get; set; }

        /// <summary>
        /// 英文后缀,th
        /// </summary>
        [Comment("英文后缀,th")]
        [Required]
        [StringLength(2)]
        public string DaySuffix { get; set; }

        /// <summary>
        /// 当前月第几周(每月第一天是第一周的第一天)
        /// </summary>
        [Comment("当前月第几周(每月第一天是第一周的第一天)")]
        public byte DOWInMonth { get; set; }

        /// <summary>
        /// 当前月第一天：2017/1/1（日期类型，需要格式化）
        /// </summary>
        [Comment("当前月第一天：2017/1/1（日期类型，需要格式化）")]
        [Column(TypeName = "date")]
        public DateTime FirstDayOfMonth { get; set; }

        /// <summary>
        /// 下个月第一天：2017-02-01（日期类型，需要格式化）
        /// </summary>
        [Comment("下个月第一天：2017-02-01（日期类型，需要格式化）")]
        [Column(TypeName = "date")]
        public DateTime FirstDayOfNextMonth { get; set; }

        /// <summary>
        /// 下一个年第一天：2018-01-01（日期类型，需要格式化）
        /// </summary>
        [Comment("下一个年第一天：2018-01-01（日期类型，需要格式化）")]
        [Column(TypeName = "date")]
        public DateTime FirstDayOfNextYear { get; set; }

        /// <summary>
        /// 当前季度第一天：2017-01-01（日期类型，需要格式化）
        /// </summary>
        [Comment("当前季度第一天：2017-01-01（日期类型，需要格式化）")]
        [Column(TypeName = "date")]
        public DateTime FirstDayOfQuarter { get; set; }

        /// <summary>
        /// 当前年第一天：2017-01-01（日期类型，需要格式化）
        /// </summary>
        [Comment("当前年第一天：2017-01-01（日期类型，需要格式化）")]
        [Column(TypeName = "date")]
        public DateTime FirstDayOfYear { get; set; }

        /// <summary>
        /// 假期名称，此字段目前未使用。礼拜日，工作日，假期字段一般会在另外的工厂日历中
        /// </summary>
        [Comment("假期名称，此字段目前未使用。礼拜日，工作日，假期字段一般会在另外的工厂日历中")]
        [StringLength(64)]
        public string HolidayText { get; set; }

        /// <summary>
        /// 是否是假期；1是
        /// </summary>
        [Comment("是否是假期；1是")]
        public bool IsHoliday { get; set; }

        /// <summary>
        /// 当前年第几周（ISO标准，每年1月4日所在周作为第一周）
        /// </summary>
        [Comment("当前年第几周（ISO标准，每年1月4日所在周作为第一周）")]
        public byte ISOWeekOfYear { get; set; }

        /// <summary>
        /// 是否是礼拜日；1是
        /// </summary>
        [Comment("是否是礼拜日；1是")]
        public bool IsWeekend { get; set; }

        /// <summary>
        /// 是否是工作日；1是
        /// </summary>
        [Comment("是否是工作日；1是")]
        public bool IsWorkday { get; set; }

        /// <summary>
        /// 当前月最后一天：2017/1/31（日期类型，需要格式化）
        /// </summary>
        [Comment("当前月最后一天：2017/1/31（日期类型，需要格式化）")]
        [Column(TypeName = "date")]
        public DateTime LastDayOfMonth { get; set; }

        /// <summary>
        /// 当前季度最后一天：2017-03-31（日期类型，需要格式化）
        /// </summary>
        [Comment("当前季度最后一天：2017-03-31（日期类型，需要格式化）")]
        [Column(TypeName = "date")]
        public DateTime LastDayOfQuarter { get; set; }

        /// <summary>
        /// 当前年最后一天：2017-12-31（日期类型，需要格式化）
        /// </summary>
        [Comment("当前年最后一天：2017-12-31（日期类型，需要格式化）")]
        [Column(TypeName = "date")]
        public DateTime LastDayOfYear { get; set; }

        /// <summary>
        /// 格式化显示（月年）：012017
        /// </summary>
        [Comment("格式化显示（月年）：012017")]
        [Required]
        [StringLength(6)]
        public string MMYYYY { get; set; }

        /// <summary>
        /// 月份
        /// </summary>
        [Comment("月份")]
        public byte Month { get; set; }

        /// <summary>
        /// 月份名称，英文：January
        /// </summary>
        [Comment("月份名称，英文：January")]
        [Required]
        [StringLength(10)]
        public string MonthName { get; set; }

        /// <summary>
        /// 格式化显示（月缩写-年）Jan2017
        /// </summary>
        [Comment("格式化显示（月缩写-年）Jan2017")]
        [Required]
        [StringLength(7)]
        public string MonthYear { get; set; }

        /// <summary>
        /// 季度：2
        /// </summary>
        [Comment("季度：2")]
        public byte Quarter { get; set; }

        /// <summary>
        /// 季度名称，英文：First
        /// </summary>
        [Comment("季度名称，英文：First")]
        [Required]
        [StringLength(6)]
        public string QuarterName { get; set; }

        /// <summary>
        /// 当前周第几天：5
        /// </summary>
        [Comment("当前周第几天：5")]
        public byte Weekday { get; set; }

        /// <summary>
        /// 星期几，英文：Wednesday
        /// </summary>
        [Comment("星期几，英文：Wednesday")]
        [Required]
        [StringLength(10)]
        public string WeekDayName { get; set; }

        /// <summary>
        /// 当前月第几周：3
        /// </summary>
        [Comment("当前月第几周：3")]
        public byte WeekOfMonth { get; set; }

        /// <summary>
        /// 当前周第几天
        /// </summary>
        [Comment("当前周第几天")]
        public byte WeekOfYear { get; set; }

        /// <summary>
        /// 当前年第几周（每年1月1日作为第一周）
        /// </summary>
        [Comment("当前年第几周（每年1月1日作为第一周）")]
        public int Year { get; set; }

        /// <summary>
        /// 格式化显示(ISO标准，年-周)：2017-01
        /// </summary>
        [Comment("格式化显示(ISO标准，年-周)：2017-01")]
        [Required]
        [StringLength(8)]
        public string YYYYISOWeek { get; set; }

        /// <summary>
        /// 格式化显示（年-月）：2017-01
        /// </summary>
        [Comment("格式化显示（年-月）：2017-01")]
        [Required]
        [StringLength(8)]
        public string YYYYMM { get; set; }

        /// <summary>
        /// 格式化显示(年-周)：2017-01
        /// </summary>
        [Comment("格式化显示(年-周)：2017-01")]
        [Required]
        [StringLength(8)]
        public string YYYYWeek { get; set; }
    }
}
