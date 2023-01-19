using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization.Users.Dto;

namespace Wimi.BtlCore.Authorization.Users
{
    public interface IUserLoginAppService : IApplicationService
    {
        Task<ListResultDto<UserLoginAttemptDto>> GetRecentUserLoginAttempts();
    }
}
