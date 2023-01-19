using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wimi.BtlCore.Order.ProductionPlans;
using Wimi.BtlCore.Timing.Utils;

namespace Wimi.BtlCore.Order.WorkOrders
{
    /// <summary>
    /// 工单表
    /// </summary>
    [Table("WorkOrders")]
    public class WorkOrder : AuditedEntity
    {
        /// <summary>
        /// 目标量
        /// </summary>
        [Comment("目标值")]
        [Required]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "目标量必须是正整数")]
        public int AimVolume { get; set; }

        /// <summary>
        /// 工单编号
        /// </summary>
        [Comment("编码")]
        [MaxLength(50)]
        [Required]
        public string Code { get; set; }

        /// <summary>
        /// 完成率
        /// </summary>  
        [Comment("完成率")]
        public decimal CompletionRate { get; set; }

        /// <summary>
        /// 次品数
        /// </summary>
        [Comment("不合格数量")]
        [Required]
        public int DefectiveCount { get; set; }

        /// <summary>
        /// 是否最后工序工单
        /// </summary>
        [Comment("是否最后工序")]
        [Required]
        public bool IsLastProcessOrder { get; set; }

        /// <summary>
        /// 产出量= 正品数+次品数
        /// </summary>
        [Comment("输出数量")]
        [Required]
        public int OutputCount { get; set; }

        /// <summary>
        /// 工序Id
        /// </summary>
        [Comment("工序key")]
        [Required]
        public int ProcessId { get; set; }

        [ForeignKey("ProductionPlanId")]
        public virtual ProductionPlan ProductionPlan { get; set; }

        /// <summary>
        /// 生产计划Id
        /// </summary>
        [Comment("生产计划key")]
        [Required]
        public int ProductionPlanId { get; set; }

        /// <summary>
        /// 投入量
        /// </summary>
        [Comment("设置值")]
        [Required]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "投放量必须是正整数")]
        public int PutVolume { get; set; }

        [Comment("合格数量")]
        [Required]
        public int QualifiedCount { get; set; }

        /// <summary>
        /// 工单状态
        /// </summary>
        [Comment("状态")]
        [Required]
        public EnumWorkOrderState State { get; set; }

        /// <summary>
        /// 关闭工单
        /// </summary>
        public void Close()
        {
            this.State = EnumWorkOrderState.Closed;

            // 最后工序工单关闭=> 计划状态根据数据更改
            if (this.IsLastProcessOrder)
            {
                this.ProductionPlan.Close();
            }
        }

        /// <summary>
        /// 登录工单更改状态
        /// </summary>
        public void Login()
        {
            this.State = EnumWorkOrderState.Producing;
            if (!this.ProductionPlan.ActualStartDate.HasValue)
            {
                this.ProductionPlan.ActualStartDate = DateTime.Now.ToCstTime();
            }

            this.ProductionPlan.Underway();
        }

        public void NotStart()
        {
            this.State = EnumWorkOrderState.NotStart;
        }

        /// <summary>
        /// 工单报工
        /// </summary>
        /// <param name="qualifiedCount"></param>
        /// <param name="defectiveCount"></param>
        public void Report(int qualifiedCount, int defectiveCount)
        {
            this.QualifiedCount = qualifiedCount;
            this.DefectiveCount = defectiveCount;
            this.OutputCount = qualifiedCount + defectiveCount;
            if (this.AimVolume > 0)
            {
                var rate = (qualifiedCount * 1.0 / this.AimVolume) * 100;
                this.CompletionRate = (decimal)Math.Round(rate, 2);
            }

            if (this.IsLastProcessOrder)
            {
                this.ProductionPlan.QualifiedCount = qualifiedCount;
                this.ProductionPlan.DefectiveCount = defectiveCount;
            }
        }
    }
}
