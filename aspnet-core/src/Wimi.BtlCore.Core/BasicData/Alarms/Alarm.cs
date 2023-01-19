using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wimi.BtlCore.BasicData.Shifts;

namespace Wimi.BtlCore.BasicData.Alarms
{
    /// <summary>
    /// 采集报警表
    /// </summary>
    [Table("Alarms")]
    public class Alarm : Entity<long>
    {
        public Alarm()
        {
            this.ShiftDetail = new ShiftDefailInfo();
        }

        /// <summary>
        /// 报警编号
        /// </summary>
        [Comment("报警编号")]
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Code { get; set; }

        /// <summary>
        /// 系统日历Key
        /// </summary>
        [Comment("系统日历Key")]
        public int? DateKey { get; set; }

        /// <summary>
        /// 持续时长（秒）
        /// </summary>
        [Comment("持续时长（秒）")]
        public decimal Duration { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Comment("结束时间")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        [Comment("设备编号")]
        [MaxLength(BtlCoreConsts.MaxLength * 2)]
        public string MachineCode { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        [Comment("设备Id")]
        public long MachineId { get; set; }

        /// <summary>
        /// 设备班次Id
        /// </summary>
        [Comment("设备班次Id")]
        public int? MachinesShiftDetailId { get; set; }

        /// <summary>
        /// 备注，报警信息手动维护在该字段
        /// </summary>
        [Comment("备注，报警信息手动维护在该字段")]
        [MaxLength(BtlCoreConsts.MaxDescLength * 5)]
        public string Memo { get; set; }

        /// <summary>
        /// 报警内容
        /// </summary>
        [Comment("报警内容")]
        [MaxLength(BtlCoreConsts.MaxDescLength * 5)]
        public string Message { get; set; }

        /// <summary>
        /// 工单Id
        /// </summary>
        [Comment("工单Id")]
        public int? OrderId { get; set; }

        /// <summary>
        /// 工件编号
        /// </summary>
        [Comment("工件编号")]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string PartNo { get; set; }

        /// <summary>
        /// 产品Id
        /// </summary>
        [Comment("产品Id")]
        public int ProductId { get; set; }

        /// <summary>
        /// 工序Id
        /// </summary>
        [Comment("工序Id")]
        public int? ProcessId { get; set; }

        /// <summary>
        /// 程序名称
        /// </summary>
        [Comment("程序名称")]
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string ProgramName { get; set; }

        /// <summary>
        /// 班次信息
        /// </summary>
        [Comment("班次信息")]
        public ShiftDefailInfo ShiftDetail { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Comment("开始时间")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [Comment("用户Id")]
        public int? UserId { get; set; }

        /// <summary>
        /// 用户班次Id
        /// </summary>
        [Comment("用户班次Id")]
        public int? UserShiftDetailId { get; set; }

        /// <summary>
        /// Mongo同步时间
        /// </summary>
        [Comment("Mongo同步时间")]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string MongoCreationTime { get; set; }
    }
}
