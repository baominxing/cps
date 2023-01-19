using System.Threading.Tasks;

namespace Wimi.BtlCore.Security
{
    public interface IPasswordComplexitySettingStore
    {
        Task<PasswordComplexitySetting> GetSettingsAsync();
    }
}
