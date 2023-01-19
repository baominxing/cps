using Abp.Runtime.Validation;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Editions.Dto
{
    public class GetEditionsInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string Filter { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "Name";
            }
        }
    }
}