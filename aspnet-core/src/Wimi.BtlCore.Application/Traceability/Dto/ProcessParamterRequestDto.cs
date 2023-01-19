using Abp.Runtime.Validation;
using System;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Traceability.Dto
{
    public class ProcessParamterRequestDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string PartNo { get; set; }

        public int? MachineId { get; set; }

        public DateTime? OperationTimeBegin { get; set; }

        public DateTime? OperationTimeEnd { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "CreationTime";
            }
        }

    }
}
