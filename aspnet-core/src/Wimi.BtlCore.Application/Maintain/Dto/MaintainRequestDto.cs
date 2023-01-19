using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;


namespace Wimi.BtlCore.Maintain.Dto
{
    [AutoMap(typeof(RepairRequest))]
    public class MaintainRequestDto : EntityDto
    {
        public string Code { get; set; }
        public int MachineId { get; set; }
        /// <summary>
        ///  申请日期
        /// </summary>
        public DateTime RequestDate { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public int RequestUserId { get; set; }
        public string RequestMemo { get; set; }
        /// <summary>
        /// 是否关机
        /// </summary>
        public bool IsShutdown { get; set; }
        public string MachineName { get; set; }
        public string MachineCode { get; set; }

        public string MachineType { get; set; }
        public string RequestUserName { get; set; }
        public EnumRepairRequestStatus Status { get; set; }

        public DateTime CreationTime { get; set; }

    }
}
