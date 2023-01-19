using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;

namespace Wimi.BtlCore.Maintain.Dto
{
    [AutoMap(typeof(MaintainPlan))]
    public class MaintainPlanDto : EntityDto
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int MachineId { get; set; }

        public string MachineCode { get; set; }

        public string MachineName { get; set; }

        public string MachineType { get; set; }

        public string Status { get; set; }

        public EnumMaintainPlanStatus EnumStatus { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int IntervalDate { get; set; }

        public int PersonInChargeId { get; set; }

        public string PersonInChargeName { get; set; }

        public string Memo { get; set; }
    }
}
