using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;

namespace Wimi.BtlCore.Maintain.Dto
{
    [AutoMap(typeof(MaintainOrder))]
    public class MaintainOrderDto : EntityDto
    {
        public string MaintainPlanCode { get; set; }

        public string MaintainPlanName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int IntervalDate { get; set; }

        public string PersonInChargeName { get; set; }

        public string MaintainPlanMemo { get; set; }

        public string Code { get; set; }

        public string Status { get; set; }

        public EnumMaintainOrderStatus EnumStatus { get; set; }

        public int MachineId { get; set; }

        public string MachineCode { get; set; }

        public string MachineName { get; set; }

        public string MachineType { get; set; }

        /// <summary>
        /// 计划保养日期
        /// </summary>
        public DateTime ScheduledDate { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 耗费时间
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 保养人
        /// </summary>
        public int? MaintainUserId { get; set; }

        public string MaintainUserName { get; set; }

        public string Memo { get; set; }
    }
}
