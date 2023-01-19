using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Shifts;

namespace Wimi.BtlCore.BasicData.Capacities
{
    /// <summary>
    /// 产量表
    /// </summary>
    [Table("Capacities")]
    public class Capacity : Entity
    {
        public Capacity()
        {
            ShiftDetail = new ShiftDefailInfo();
            MachineGroupInfo = new MachineGroupInfo();
        }

        /// <summary>
        /// 累积计数器
        /// </summary>
        [Comment("累积计数器")]
        [Required]
        public decimal AccumulateCount { get; set; }

        /// <summary>
        /// 系统日历key
        /// </summary>
        [Comment("系统日历key")]
        public int? DateKey { get; set; }

        /// <summary>
        /// 加工时长(秒)
        /// </summary>
        [Comment("加工时长(秒)")]
        public long Duration { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Comment("结束时间")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 是否有效记录
        /// </summary>
        [Comment("是否有效记录")]
        [Required]
        public bool IsValid { get; set; }

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
        /// 设备组信息
        /// </summary>
        [Comment("设备组信息")]
        [NotMapped]
        public MachineGroupInfo MachineGroupInfo { get; set; }

        [NotMapped]
        public int ShiftSolutionItemId { get; set; }

        /// <summary>
        /// 设备班次Id
        /// </summary>
        [Comment("设备班次Id")]
        public int? MachinesShiftDetailId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Comment("备注")]
        [MaxLength(BtlCoreConsts.MaxDescLength * 2)]
        public string Memo { get; set; }

        /// <summary>
        /// 工单Id
        /// </summary>
        [Comment("工单Id")]
        public int? OrderId { get; set; }

        /// <summary>
        /// 原始计数器
        /// </summary>
        [Comment("原始计数器")]
        public decimal OriginalCount { get; set; }

        /// <summary>
        /// 零件码
        /// </summary>
        [Comment("零件码")]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string PartNo { get; set; }

        /// <summary>
        /// 产品Id
        /// </summary>
        [Comment("产品Id")]
        public int ProductId { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [Comment("产品名称")]
        [MaxLength(50)]
        public string ProductName { get; set; }

        /// <summary>
        /// 工序Id
        /// </summary>
        [Comment("工序Id")]
        public int? ProcessId { get; set; }

        /// <summary>
        ///     程序名称
        /// </summary>
        [Comment("程序名称")]
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string ProgramName { get; set; }

        /// <summary>
        ///     循环倍率
        /// </summary>
        [Comment("循环倍率")]
        public decimal Rate { get; set; }

        /// <summary>
        /// 班次的信息
        /// </summary>
        [Comment("班次的信息")]
        public ShiftDefailInfo ShiftDetail { get; set; }

        /// <summary>
        ///     开始时间
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
        ///     产量
        /// </summary>
        [Comment("产量")]
        [Required]
        public decimal Yield { get; set; }

        /// <summary>
        /// 是否是产线产出
        /// </summary>
        [Comment("是否是产线产出")]
        public bool IsLineOutput { get; set; }

        /// <summary>
        /// 工件状态
        /// </summary>
        [Comment("工件状态")]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Tag { get; set; }

        /// <summary>
        /// 质量
        /// </summary>
        [Comment("质量")]
        public bool? Qualified { get; set; }

        /// <summary>
        /// 生产计划Id
        /// </summary>
        [Comment("生产计划Id")]
        public int? PlanId { get; set; }

        /// <summary>
        /// 生产计划名称
        /// </summary>
        [Comment("生产计划名称")]
        [MaxLength(50)]
        public string PlanName { get; set; }

        /// <summary>
        /// 生产计划目标产量
        /// </summary>
        [Comment("生产计划目标产量")]
        public int? PlanAmount { get; set; }

        /// <summary>
        /// 记录DmpId，用于内部计算使用
        /// </summary>
        [Comment("记录DmpId，用于内部计算使用")]
        public Guid DmpId { get; set; }

        /// <summary>
        /// 连接前面一笔记录的DmpId
        /// </summary>
        [Comment("连接前面一笔记录的DmpId")]
        public Guid? PreviousLinkId { get; set; }

        /// <summary>
        /// 产线产量是否下线
        /// </summary>
        [Comment("产线产量是否下线")]
        public bool IsLineOutputOffline { get; set; }

        /// <summary>
        /// Mongo同步时间
        /// </summary>
        [Comment("Mongo同步时间")]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string MongoCreationTime { get; set; }
    }
}
