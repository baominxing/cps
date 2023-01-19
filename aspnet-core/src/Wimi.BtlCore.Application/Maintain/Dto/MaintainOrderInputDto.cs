using Abp.AutoMapper;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Maintain.Dto
{
    [AutoMap(typeof(MaintainOrder))]
    public class MaintainOrderInputDto : PagedSortedAndFilteredInputDto,IShouldNormalize
    {
        public int Id { get; set; }

        public string MaintainPlanCode { get; set; }

        public string Code { get; set; }

        public List<EnumMaintainOrderStatus> StatusList { get; set; } = new List<EnumMaintainOrderStatus>();

        public EnumMaintainOrderStatus Status { get; set; }

        public int MachineId { get; set; }

        public string MachineCode { get; set; }

        public string MachineName { get; set; }

        public string MachineType { get; set; }

        /// <summary>
        /// 计划保养日期
        /// </summary>
        public DateTime ScheduledDate { get; set; }

        /// <summary>
        /// 实际保养日期
        /// </summary>
        public DateTime? MaintainDate { get; set; }

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

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "Code";
            }
        }
    }
}
