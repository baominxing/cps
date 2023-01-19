using System;

using Abp.Extensions;
using Abp.Runtime.Validation;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Order.MachineReport.Dtos
{
    public class MachineReportDefectiveReasonRequestDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int? MacineId { get; set; }

        public DateTime? Date { get; set; }

        public void Normalize()
        {
            if (this.Sorting.IsNullOrEmpty())
            {
                this.Sorting = "MachineId,Hour";
            }
        }
    }
}
