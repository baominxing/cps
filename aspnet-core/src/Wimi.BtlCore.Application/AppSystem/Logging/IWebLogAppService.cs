using Abp.Application.Services;

using Wimi.BtlCore.AppSystem.Logging.Dto;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.AppSystem.Logging
{
    public interface IWebLogAppService : IApplicationService
    {
        // ReSharper disable once StyleCop.WM0004
        FileDto DownloadWebLogs();

        // ReSharper disable once StyleCop.WM0004
        GetLatestWebLogsOutputDto GetLatestWebLogs();
    }
}