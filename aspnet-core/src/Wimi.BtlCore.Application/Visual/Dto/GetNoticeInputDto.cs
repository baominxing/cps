namespace Wimi.BtlCore.Visual.Dto
{
    using Abp.AutoMapper;
    using Abp.Runtime.Validation;

    using Wimi.BtlCore.Dto;
    using Wimi.BtlCore.Notices;

    [AutoMap(typeof(Notice))]
    public class GetNoticeInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string Content { get; set; }

        public int? Id { get; set; }

        public bool IsActive { get; set; }

        public string RootDeviceGroupCode { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "Id";
            }
        }
    }
}