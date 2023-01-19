namespace Wimi.BtlCore.Dto
{
    public class PagedSortedAndFilteredInputDto : PagedAndSortedInputDto
    {
        public PagedSortedAndFilteredInputDto()
        {
            this.Search = new DatatablesSearchInputDto();
        }

        public DatatablesSearchInputDto Search { get; set; }
    }
}