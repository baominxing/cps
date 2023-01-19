using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Weixin.Dto
{

    public class FindUserInputDto : PagedSortedAndFilteredInputDto
    {
        public int? NotificationTypeId { get; set; }
    }
}