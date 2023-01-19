using Abp.Runtime.Validation;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Shifts.Dto
{
    public class ShiftSolutionInputDto:PagedSortedAndFilteredInputDto,IShouldNormalize
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "Id";
            }
        }
    }
}