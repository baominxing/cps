using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wimi.BtlCore.Order.Crafts;
using Wimi.BtlCore.Order.Products;
using Wimi.BtlCore.Order.WorkOrders;
using Wimi.BtlCore.Timing.Utils;

namespace Wimi.BtlCore.Order.ProductionPlans
{
    public class ProductionPlan : FullAuditedEntity
    {
        public ProductionPlan()
        {
            this.WorkOrders = new HashSet<WorkOrder>();
        }

        /// <summary>
        /// 实际结束时间
        /// </summary>
        [Comment("实际结束时间")]
        public DateTime? ActualEndDate { get; set; }

        /// <summary>
        /// 实际开始时间
        /// </summary>
        [Comment("实际开始时间")]
        public DateTime? ActualStartDate { get; set; }

        [Comment("目标值")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "目标量必须是正整数")]
        public int AimVolume { get; set; }

        [Comment("客户端名称")]
        [MaxLength(50)]
        public string ClientName { get; set; }

        [Comment("编码")]
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [ForeignKey("CraftId")]
        public virtual Craft Craft { get; set; }

        [Comment("工艺key")]
        public int CraftId { get; set; }

        [Comment("备注")]
        public string Memo { get; set; }

        /// <summary>
        /// 次品数
        /// </summary>
        [Comment("不良数量")]
        public int DefectiveCount { get; set; }

        [NotMapped]
        public string DisplayState => this.ProductionPlanState.ToString();

        [Comment("结束时间")]
        public DateTime EndDate { get; set; }

        [Comment("工序编码")]
        [MaxLength(50)]
        public string OrderCode { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Comment("产品key")]
        public int ProductId { get; set; }

        [Comment("生产计划状态")]
        public ProductionPlanStates ProductionPlanState { get; set; }

        [Comment("设置值")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "投放量必须是正整数")]
        public int PutVolume { get; set; }

        /// <summary>
        /// 正品数
        /// </summary>
        [Comment("合格数量")]
        public int QualifiedCount { get; set; }

        [Comment("开始时间")]
        public DateTime StartDate { get; set; }

        [Comment("单位")]
        [MaxLength(10)]
        public string Unit { get; set; }

        public virtual ICollection<WorkOrder> WorkOrders { get; set; }

        public void BreakOff()
        {
            this.ProductionPlanState = ProductionPlanStates.BreakOff;
        }

        public void Close()
        {
            var planState = this.QualifiedCount >= this.AimVolume
                                ? ProductionPlanStates.Completed
                                : ProductionPlanStates.BreakOff;

            this.ProductionPlanState = planState;
            if (!this.ActualStartDate.HasValue)
            {
                this.ActualStartDate = DateTime.Now.ToCstTime();
            }

            this.ActualEndDate = DateTime.Now.ToCstTime();
        }

        public void Complete()
        {
            this.ProductionPlanState = ProductionPlanStates.Completed;
        }

        public bool IsPrepared()
        {
            return this.ProductionPlanState == ProductionPlanStates.Prepared;
        }

        public void Prepare()
        {
            this.ProductionPlanState = ProductionPlanStates.Prepared;
        }

        public void Underway()
        {
            this.ProductionPlanState = ProductionPlanStates.Underway;
        }
    }
}
