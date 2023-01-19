using System.Threading.Tasks;

using Abp.Application.Services;

using Wimi.BtlCore.Configuration.Host.Dto;

namespace Wimi.BtlCore.Configuration.Host
{
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettings();

        Task SendTestEmail(SendTestEmailInputDto input);

        Task UpdateAllSettings(HostSettingsEditDto input);
    }
}