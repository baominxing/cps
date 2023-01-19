using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Wimi.BtlCore.Configuration.Dto;

namespace Wimi.BtlCore.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : BtlCoreAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
