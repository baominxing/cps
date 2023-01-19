namespace Wimi.BtlCore.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class DatatablesPagedInputDto : PagedInputDto, IDrawCounter
    {
        public DatatablesPagedInputDto()
        {
            this.Length = AppConsts.DefaultPageSize;
        }

        public int Draw { get; set; }

        [Range(1, AppConsts.MaxPageSize)]
        public int Length
        {
            get
            {
                return this.MaxResultCount;
            }

            set
            {
                this.MaxResultCount = value;
            }
        }

        [Range(0, int.MaxValue)]
        public int Start
        {
            get
            {
                return this.SkipCount;
            }

            set
            {
                this.SkipCount = value;
            }
        }
    }
}