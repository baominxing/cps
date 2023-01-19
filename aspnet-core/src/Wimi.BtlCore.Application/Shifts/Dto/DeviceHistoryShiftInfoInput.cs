using Abp.Runtime.Validation;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Shifts.Dto
{
    public class DeviceHistoryShiftInfoInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int DeviceId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "StartDate DESC";
            }
        }
    }
}