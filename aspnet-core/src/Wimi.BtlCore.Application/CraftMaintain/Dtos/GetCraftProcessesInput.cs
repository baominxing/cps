using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.CraftMaintain.Dtos
{
    public class GetCraftProcessesInput : PagedAndFilteredInputDto
    {
        public int? CraftId { get; set; }
    }
}
