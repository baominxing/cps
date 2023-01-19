using Abp.AutoMapper;
using System;
using Wimi.BtlCore.Maintain;
using Wimi.BtlCore.Maintain.Dto;

namespace Wimi.BtlCore.Web.Models.Maintain.MaintainRepair
{
    [AutoMap(typeof(MaintainRepairDto))]
    public class MaintainRepairViewModel
    {
        public int Id { get; set; }
        public EnumRepairRequestStatus Status { get; set; }
        public bool IsEditMode { get; set; }
        public string Code { get; set; }
        public string RequestPlanDate { get; set; }
        public string MachineType { get; set; }
        public string ShutDownStatus { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal Cost { get; set; }
        public string RepairMemo { get; set; }
        //查看
        public string MachineCode { get; set; }
        public string RequestUserName { get; set; }
        public string RequestMemo { get; set; }
    }
}
