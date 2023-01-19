using System.Threading.Tasks;
using Wimi.BtlCore.Configuration.Dto;

namespace Wimi.BtlCore.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
