using Abp.Runtime.Validation;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Order.DefectiveReasons.Dtos
{
    public class DefectiveReasonFilterDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int PartId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "Id";
            }
        }
    }
}
