using Abp.AutoMapper;
using System;

namespace Wimi.BtlCore.Maintain.Dto
{
    [AutoMap(typeof(RepairRequest))]
   public class MaintainRepairDto
    {
        public string Code { get; set; }

        public int MachineId { get; set; }

        public EnumRepairRequestStatus Status { get; set; }
     
        public DateTime RequestDate { get; set; }
        
        public int RequestUserId { get; set; }
        
        public string RequestMemo { get; set; }

        /// <summary>
        /// 维修人
        /// </summary>
        public int? RepairUserId { get; set; }

        /// <summary>
        /// 是否关机
        /// </summary>
        public bool IsShutdown { get; set; }

        /// <summary>
        /// 维修开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 维修结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 耗费时间
        /// </summary>
        public decimal Cost { get; set; }
        public string RepairMemo { get; set; }
        public string RequestUserName { get; set; }
        public string MachineCode { get; set; }
        public string MachineType { get; set; }
    }
}
