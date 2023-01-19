using Abp.Extensions;
using Abp.Runtime.Validation;
using System;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Carton.CartonTraceability.Dtos
{
    public class CartonTraceabilityRequestDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string CartonNo { get; set; }

        public string PartNo { get; set; }

        public int DeviceGroupId { get; set; }

        public void Normalize()
        {
            if (this.Sorting.IsNullOrWhiteSpace())
            {
                this.Sorting = "CreationTime desc";
            }
        }
    }
}
