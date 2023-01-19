using Abp.AutoMapper;
using System;
using Wimi.BtlCore.Maintain;
using Wimi.BtlCore.Maintain.Dto;

namespace Wimi.BtlCore.Web.Models.Maintain.MaintainRequest
{
    [AutoMap(typeof(MaintainRequestDto))]
    public class MaintainRequestViewModel
    {
        public int Id { get; set; }
        public bool IsEditMode { get; set; }
        public string Code { get; set; }
        public long RequestUserId { get; set; }
        public int MachineId { get; set; }
        public string MachineType { get; set; }
        public bool IsShutDown { get; set; }
        public DateTime? RequestDate { get; set; }
        public string RequestMemo { get; set; }
        public EnumRepairRequestStatus Status { get; set; }
        //查看
        public string MachineName { get; set; }
        public string RequestUserName { get; set; }
        public string MachineCode { get; set; }
    }
}
