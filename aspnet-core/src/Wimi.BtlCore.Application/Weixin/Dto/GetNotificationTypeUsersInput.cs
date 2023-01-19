namespace Wimi.BtlCore.Weixin.Dto
{
    using System.ComponentModel.DataAnnotations;

    using Abp.Runtime.Validation;
    using Wimi.BtlCore.Dto;

    public class GetNotificationTypeUsersInputDto : PagedAndSortedInputDto, IShouldNormalize
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "Name";
            }
        }
    }
}