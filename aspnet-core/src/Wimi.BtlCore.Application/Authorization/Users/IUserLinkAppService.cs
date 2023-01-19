using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization.Users.Dto;

namespace Wimi.BtlCore.Authorization.Users
{
    public interface IUserLinkAppService : IApplicationService
    {
        Task<PagedResultDto<LinkedUserDto>> GetLinkedUsers(GetLinkedUsersInputDto input);

        Task<ListResultDto<LinkedUserDto>> GetRecentlyUsedLinkedUsers();

        Task LinkToUser(LinkToUserInputDto linkToUserInput);

        Task UnlinkUser(UnlinkUserInputDto input);
    }
}
