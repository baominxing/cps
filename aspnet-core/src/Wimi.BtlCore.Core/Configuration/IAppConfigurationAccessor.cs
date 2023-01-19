using Microsoft.Extensions.Configuration;

namespace Wimi.BtlCore.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
