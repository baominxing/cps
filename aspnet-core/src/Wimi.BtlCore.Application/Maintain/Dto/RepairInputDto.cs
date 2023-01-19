using Abp.Application.Services.Dto;
using System;

namespace Wimi.BtlCore.Maintain.Dto
{
    public  class RepairInputDto:EntityDto
    {
        public bool IsEditMode { get; set; }
        public decimal Cost { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string RepairMemo { get; set; }
        public EnumRepairRequestStatus Status { get; set; }
    }
}
