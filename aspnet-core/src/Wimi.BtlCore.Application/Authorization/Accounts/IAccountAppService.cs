using System.Threading.Tasks;
using Abp.Application.Services;
using Wimi.BtlCore.Authorization.Accounts.Dto;

namespace Wimi.BtlCore.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
