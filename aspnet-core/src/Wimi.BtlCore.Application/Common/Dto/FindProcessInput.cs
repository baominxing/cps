using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Common.Dto
{

    public class FindProcessInputDto : PagedAndFilteredInputDto
    {
        public int CraftId { get; set; }
    }
}