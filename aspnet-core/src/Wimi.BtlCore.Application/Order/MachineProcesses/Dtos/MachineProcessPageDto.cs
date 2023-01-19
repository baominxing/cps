using Abp.Extensions;
using Abp.Runtime.Validation;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Order.MachineProcesses.Dtos
{
    public class MachineProcessPageDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int?  MachineId { get; set; }

        public int? ProductId { get; set; }

        public void Normalize()
        {
            if (this.Sorting.IsNullOrEmpty())
            {
                this.Sorting = "EndTime";
            }
        }
    }
}
