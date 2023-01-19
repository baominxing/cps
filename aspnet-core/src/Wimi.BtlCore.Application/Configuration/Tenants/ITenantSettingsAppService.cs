using System.Threading.Tasks;

using Abp.Application.Services;
using Wimi.BtlCore.Configuration.Tenants.Dto;

namespace Wimi.BtlCore.Configuration.Tenants
{
    public interface ITenantSettingsAppService : IApplicationService
    {
        Task<TenantSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(TenantSettingsEditDto input);
    }
}