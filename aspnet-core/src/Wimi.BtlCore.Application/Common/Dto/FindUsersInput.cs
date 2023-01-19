namespace Wimi.BtlCore.Common.Dto
{
    using Wimi.BtlCore.Dto;

    public class FindUsersInputDto : PagedSortedAndFilteredInputDto
    {
        public int? TenantId { get; set; }
    }
}