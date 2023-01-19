using Abp.Runtime.Validation;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Traceability.Dto
{
    public class QueryTraceLineFlowSettingDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int? DeviceGroupId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Code asc";
            }
        }
    }
}
