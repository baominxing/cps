using System;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wimi.BtlCore.Timing.Utils;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Shifts;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.BasicData.States
{
    /// <summary>
    ///     采集状态表
    /// </summary>
    [Table("States")]
    public class State : Entity<long>
    {
        public State()
        {
            this.ShiftDetail = new ShiftDefailInfo();
            this.MachineGroupInfo = new MachineGroupInfo();
        }

        /// <summary>
        ///     状态编码
        /// </summary>
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("状态编号")]
        public string Code { get; set; }

        /// <summary>
        /// 系统日历key
        /// </summary>
        [Comment("系统日历 20230106")]
        public int? DateKey { get; set; }

        /// <summary>
        ///     持续时长（秒）
        /// </summary>
        [Comment("持续时长（秒）")]
        public decimal Duration { get; set; }

        /// <summary>
        ///     结束时间
        /// </summary>
        //[Index("IX_States", 3)]
        [Comment("结束时间")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        ///     设备机器编号
        /// </summary>
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength * 2)]
        [Comment("设备编号")]
        public string MachineCode { get; set; }

        /// <summary>
        ///     机器Id
        /// </summary>
        //[Index("IX_States",1)]
        [Comment("设备Id")]
        public long MachineId { get; set; }

        [NotMapped]
        public MachineGroupInfo MachineGroupInfo { get; set; }

        /// <summary>
        /// 设备班次key
        /// </summary>
        //[Index("IX_States", 4)]
        [Comment("设备班次Id")]
        public int? MachinesShiftDetailId { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxDescLength * 2)]
        [Comment("备注")]
        public string Memo { get; set; }

        /// <summary>
        ///     状态名称
        /// </summary>
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("状态名称")]
        public string Name { get; set; }

        /// <summary>
        /// 工单key
        /// </summary>
        [Comment("工单Id")]
        public int? OrderId { get; set; }

        /// <summary>
        /// 零件码
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("零件码")]
        public string PartNo { get; set; }

        [Comment("产品Id")]
        public int ProductId { get; set; }

        /// <summary>
        /// 工序key
        /// </summary>
        [Comment("工序Id")]
        public int? ProcessId { get; set; }

        /// <summary>
        /// 程序号
        /// </summary>
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        [Comment("程序号")]
        public string ProgramName { get; set; }

        /// <summary>
        /// 班次的信息
        /// </summary>
        [Comment("班次的信息")]
        public ShiftDefailInfo ShiftDetail { get; set; }

        /// <summary>
        ///     开始时间
        /// </summary>
        //[Index("IX_States", 2)]
        [Comment("班次开始时间")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 用户key
        /// </summary>
        [Comment("用户Id")]
        public int? UserId { get; set; }

        /// <summary>
        /// 用户班次key
        /// </summary>
        [Comment("用户班次Id")]
        public int? UserShiftDetailId { get; set; }

        [Comment("是否切班产生")]
        public bool IsShiftSplit { get; set; }


        public DateTime? LastModificationTime { get; set; }

        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 状态标识
        /// </summary>
        [Comment("唯一标识(GUID)")]
        public Guid DmpId { get; set; }

        /// <summary>
        /// 连接前面一笔记录的DmpId
        /// </summary>
        [Comment("连接前面一笔记录的DmpId")]
        public Guid? PreviousLinkId { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength)]
        [Comment("创建时间 记录在MongoDb")]
        public string MongoCreationTime { get; set; }

        public void StartCurrentShift(int machinesShiftDetailId, DateTime startTime, ShiftDefailInfo shiftDefail)
        {
            this.ShiftDetail = shiftDefail ?? new ShiftDefailInfo();

            this.Id = 0;
            this.StartTime = startTime;
            this.EndTime = null;
            this.Duration = 0;
            this.DateKey = Convert.ToInt32(DateTime.Today.ToString("yyyyMMdd"));
            this.MachinesShiftDetailId = machinesShiftDetailId;
            this.IsShiftSplit = true;
            this.CreationTime = DateTime.Now;
            this.PreviousLinkId = this.DmpId;
            this.DmpId = Guid.NewGuid();
        }

        public void StartStaffOnline(long userId, int shiftId)
        {
            this.Id = 0;
            this.StartTime = DateTime.Now;
            this.EndTime = null;
            this.Duration = 0;
            this.IsShiftSplit = true;
            this.CreationTime = DateTime.Now;
            this.PreviousLinkId = this.DmpId;
            this.DmpId = Guid.NewGuid();
            this.UserId = (int)userId;
            this.UserShiftDetailId = shiftId;
        }

        public void EndShift(DateTime endTime)
        {
            this.EndTime = endTime;
            this.LastModificationTime = DateTime.Now.ToCstTime();
            this.Duration = this.EndTime < this.StartTime ? 0 : (decimal)(this.EndTime.Value - this.StartTime.Value).TotalSeconds;
        }

        public void StartCurrentNaturalDay(ShiftDefailInfo shiftDetail, int? newShiftId)
        {
            this.Id = 0;
            this.StartTime = DateTime.Today;
            this.EndTime = null;
            this.Duration = 0;
            this.DateKey = Convert.ToInt32(DateTime.Today.ToString("yyyyMMdd"));
            this.CreationTime = DateTime.Now;
            this.IsShiftSplit = true;
            this.LastModificationTime = null;
            this.PreviousLinkId = this.DmpId;
            this.DmpId = Guid.NewGuid();
            this.ShiftDetail = shiftDetail;
            if (newShiftId != null)
            {
                this.MachinesShiftDetailId = newShiftId;
            }
        }
    }
}
