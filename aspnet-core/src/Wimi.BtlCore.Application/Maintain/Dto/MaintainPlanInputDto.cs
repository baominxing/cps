using Abp.AutoMapper;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Maintain.Dto
{
    [AutoMap(typeof(MaintainPlan))]
    public class MaintainPlanInputDto : PagedSortedAndFilteredInputDto,IShouldNormalize
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int MachineId { get; set; }

        public List<EnumMaintainPlanStatus> StatusList { get; set; } = new List<EnumMaintainPlanStatus>();

        public EnumMaintainPlanStatus Status { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int IntervalDate { get; set; }

        public int PersonInChargeId { get; set; }

        public string Memo { get; set; }

        public bool GenerateNewMaintainCode { get; set; } = true;

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "Code";
            }
        }
    }
}
