using System.ComponentModel.DataAnnotations;

using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using Wimi.BtlCore.Order.WorkOrders;

namespace Wimi.BtlCore.Order
{
    [AutoMap(typeof(WorkOrders.WorkOrder))]
    public class WorkOrderDto : AuditedEntity
    {
        /// <summary>
        /// 目标量
        /// </summary>
        public int AimVolume { get; set; }

        /// <summary>
        /// 循环倍率
        /// </summary>
        public int? CirculationRate { get; set; }

        /// <summary>
        /// 工单编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 完成率
        /// </summary>  
        public decimal CompletionRate { get; set; }

        /// <summary>
        /// 工艺编号
        /// </summary>
        public string CraftCode { get; set; }

        /// <summary>
        /// 工艺名称
        /// </summary>
        public string CraftName { get; set; }

        /// <summary>
        /// 次品数
        /// </summary>
        public int DefectiveCount { get; set; }

        /// <summary>
        /// 是否最后工序工单
        /// </summary>
        public bool IsLastProcessOrder { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderCode { get; set; }

        /// <summary>
        /// 产出量= 正品数+次品数
        /// </summary>
        [Required]
        public int OutputCount { get; set; }

        /// <summary>
        /// 生产计划状态
        /// </summary>
        public string PlanState { get; set; }

        /// <summary>
        /// 工序编号
        /// </summary>
        public string ProcessCode { get; set; }

        /// <summary>
        /// 工序Id
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 工序顺序
        /// </summary>
        public int ProcessOrderSeq { get; set; }

        /// <summary>
        /// 产品代码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 计划编号
        /// </summary>
        public string ProductionPlanCode { get; set; }

        /// <summary>
        /// 生产计划Id
        /// </summary>
        public int ProductionPlanId { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 投入量
        /// </summary>
        public int PutVolume { get; set; }

        /// <summary>
        /// 正品数
        /// </summary>
        public int QualifiedCount { get; set; }

        /// <summary>
        /// 标准用时(秒)
        /// </summary>
        public decimal? StandardTime { get; set; }

        /// <summary>
        /// 工单状态
        /// </summary>
        public EnumWorkOrderState State { get; set; }

        /// <summary>
        /// 状态名称
        /// </summary>
        public string StateName { get; set; }
    }
}
