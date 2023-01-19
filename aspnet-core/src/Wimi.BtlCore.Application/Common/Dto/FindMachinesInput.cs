using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Common.Dto
{

    public class FindMachinesInputDto : PagedAndFilteredInputDto
    {
        public int? DeviceGroupId { get; set; }

        public int? TenantId { get; set; }

        public int? DmpId { get; set; }
    }
}