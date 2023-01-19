using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;

namespace Wimi.BtlCore.Plan.ProcessPlans.Dtos
{
    [AutoMapTo(typeof(ProcessPlan))]
    public class CreatePlanDto : EntityDto
    {
        public CreatePlanDto()
        {
            YieldSummaryType = EnumYieldSummaryType.ByYieldCounter;
            TargetType = EnumTargetDimension.ByDay;
            ShiftTarget = new List<ShiftItemDto>();
        }
        //计划名称
        public string PlanName { get; set; }
        //产品
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        //计划生产量
        public int PlanAmount { get; set; }

        //设备组
        public int DeviceGroupId { get; set; }
        //时间范围是否选中
        public bool IsTimeRangeSelect { get; set; }
        //计划开始时间
        public DateTime? PlanStartTime { get; set; }
        //计划结束时间
        public DateTime? PlanEndTime { get; set; }
        //计划产量达标后自动关闭
        public bool IsAutoFinishCurrentPlan { get; set; }
        //自动开启后续计划
        public bool IsAutoStartNextPlan { get; set; }
        //目标量维度
        public EnumTargetDimension TargetType { get; set; }
        //目标量
        public int TargetAmount { get; set; }
        //班次目标量
        public List<ShiftItemDto> ShiftTarget { get; set; }
        //产量计算方式
        public EnumYieldSummaryType YieldSummaryType { get; set; }
        //产量计数器设备
        public int YieldCounterMachineId { get; set; }

        public string ShiftTargetJson { get; set; }
    }
}
