using System.Threading.Tasks;
using Abp.Application.Services;
using Wimi.BtlCore.AppSystem.Sessions.Dto;

namespace Wimi.BtlCore.AppSystem.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
