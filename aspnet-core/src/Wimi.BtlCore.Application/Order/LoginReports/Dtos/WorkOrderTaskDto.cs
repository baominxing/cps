namespace Wimi.BtlCore.Order.LoginReports.Dtos
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Abp.AutoMapper;
    using Abp.Domain.Entities;
    using Wimi.BtlCore.Order.WorkOrders;

    [AutoMap(typeof(WorkOrderTasks))]
    public class WorkOrderTaskDto : Entity
    {
        /// <summary>
        /// 目标量
        /// </summary>
        public int AimVolume { get; set; }

        /// <summary>
        /// 次品数
        /// </summary>
        [Required]
        public int DefectiveCount { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        [Required]
        public int MachineId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// 产出量
        /// </summary>
        [Required]
        public int OutputCount { get; set; }

        /// <summary>
        /// 投放量
        /// </summary>
        public int PutVolume { get; set; }

        /// <summary>
        /// 合格数量/正品数
        /// </summary>
        [Required]
        public int QualifiedCount { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 人员Id
        /// </summary>
        [Required]
        public long UserId { get; set; }

        /// <summary>
        /// 人员姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 工单Id
        /// </summary>
        [Required]
        public int WorkOrderId { get; set; }
    }
}
