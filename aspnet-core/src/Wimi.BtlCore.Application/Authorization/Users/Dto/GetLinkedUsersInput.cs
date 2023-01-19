using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace Wimi.BtlCore.Authorization.Users.Dto
{
    public class GetLinkedUsersInputDto : IPagedResultRequest, ISortedResultRequest, IShouldNormalize
    {
        public int Length { get; set; }
        public int MaxResultCount { get; set; }

        public int SkipCount { get; set; }

        public string Sorting { get; set; }

        public int Start { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Username";
            }
        }
    }
}