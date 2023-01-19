using Abp.Runtime.Validation;
using System;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Maintain.Dto
{
    public class MainRequestFilterDto: PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        //工单号
        public string Code { get; set; }
        public string MachineId { get; set; }
        public int Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "RequestDate";
            }
        }
    }
}
