using Abp.Domain.Entities.Auditing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wimi.BtlCore.Timing.Utils;

namespace Wimi.BtlCore.Plan
{
    [Table("ProcessPlans")]
    public class ProcessPlan : FullAuditedEntity
    {
        public ProcessPlan()
        {
            ShiftTarget = new List<PlanTarget>();
            TargetType = EnumTargetDimension.ByDay;
            Status = EnumPlanStatus.New;
            YieldSummaryType = EnumYieldSummaryType.ByYieldCounter;
            if (string.IsNullOrEmpty(PlanCode))
            {
                PlanCode = DateTime.Now.ToCstTime().ToString("yyyyMMddHHmmss");
            }
        }
        //计划名称
        [Comment("计划名称")]
        [MaxLength(50)]
        public string PlanName { get; set; }

        [Comment("计划编号")]
        [MaxLength(50)]
        public string PlanCode { get; set; }

        //产品
        [Comment("产品ID")]
        public int ProductId { get; set; }

        //产品名称
        [Comment("产品名称")]
        [MaxLength(50)]
        public string ProductName { get; set; }

        //计划生产量
        [Comment("计划产量")]
        public int PlanAmount { get; set; }

        //设备组
        [Comment("设备组ID")]
        public int DeviceGroupId { get; set; }

        //时间范围是否选中
        [Comment("时间范围是否选中")]
        public bool IsTimeRangeSelect { get; set; }

        //计划开始时间
        [Comment("计划开始时间")]
        public DateTime? PlanStartTime { get; set; }

        //计划结束时间
        [Comment("计划截止时间")]
        public DateTime? PlanEndTime { get; set; }

        [Comment("实际开始时间")]
        public DateTime? RealStartTime { get; set; }

        [Comment("实际截止时间")]
        public DateTime? RealEndTime { get; set; }

        [Comment("暂停时间")]
        public DateTime? PauseTime { get; set; }

        //计划产量达标后自动关闭
        [Comment("计划生产量达标后自动关闭计划")]
        public bool IsAutoFinishCurrentPlan { get; set; }

        //自动开启后续计划
        [Comment("自动开启后续计划")]
        public bool IsAutoStartNextPlan { get; set; }

        //目标量维度
        [Comment("目标量维度")]
        public EnumTargetDimension TargetType { get; set; }

        //目标量
        [Comment("目标量")]
        public int TargetAmount { get; set; }

        //班次目标量{"班次ID":数量}
        public List<PlanTarget> ShiftTarget { get; set; }

        //产量计算方式
        [Comment("产量计算方式")]
        public EnumYieldSummaryType YieldSummaryType { get; set; }

        //产量计数器设备
        [Comment("产量计数器设备")]
        public int YieldCounterMachineId { get; set; }

        [Comment("计划状态")]
        public EnumPlanStatus Status { get; set; }

        [Comment("已完成量")]
        public int ProcessAmount { get; set; }

        public void Verify()
        {

            if (string.IsNullOrWhiteSpace(PlanName))
            {
                throw new UserFriendlyException("计划名称不能为空");
            }
            else if (ProductId <= 0)
            {
                throw new UserFriendlyException("请选择产品");
            }
            else if (DeviceGroupId <= 0)
            {
                throw new UserFriendlyException("请选择设备组");
            }
            else if (YieldSummaryType == EnumYieldSummaryType.ByYieldCounter && YieldCounterMachineId <= 0)
            {
                throw new UserFriendlyException("请选择产量统计设备");
            }
            else if (TargetType == EnumTargetDimension.ByShift && ShiftTarget.Count <= 0)
            {
                throw new UserFriendlyException("请选择班次");

            }

        }
        public bool CanEdit()
        {
            if (Status != EnumPlanStatus.Complete)
            {
                return true;
            }
            return false;
        }
        public bool CanDelete()
        {
            if (Status == EnumPlanStatus.New)
            {
                return true;
            }
            return false;
        }
        public bool CanUpdateStatus(EnumPlanStatus targetStatus)
        {
            if (Status == targetStatus)
            {
                return true;
            }

            switch (this.Status)
            {
                case EnumPlanStatus.New:
                    if (targetStatus == EnumPlanStatus.InProgress) return true;
                    break;
                case EnumPlanStatus.InProgress:
                    if ((targetStatus == EnumPlanStatus.Complete) || (targetStatus == EnumPlanStatus.Pause)) return true;
                    break;
                case EnumPlanStatus.Pause:
                    if (targetStatus == EnumPlanStatus.Complete || targetStatus == EnumPlanStatus.InProgress) return true;
                    break;

                default: return false;
            }

            return false;
        }
    }
    [Flags]
    public enum EnumPlanStatus
    {
        //新增
        New,
        //暂停
        Pause,
        //进行中
        InProgress,
        //结束
        Complete,
        //维度变更
        AutoComplete,
    }
    [Flags]
    public enum EnumTargetDimension
    {
        ByDay = 0,
        ByWeek = 1,
        ByMonth = 2,
        ByYear = 3,
        ByShift = 4
    }
    [Flags]
    public enum EnumYieldSummaryType
    {
        ByTraceOffline,
        ByYieldCounter
    }
}
