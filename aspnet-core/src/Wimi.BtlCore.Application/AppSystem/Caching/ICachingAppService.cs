using System.Threading.Tasks;

using Abp.Application.Services;
using Abp.Application.Services.Dto;

using Wimi.BtlCore.AppSystem.Caching.Dto;

namespace Wimi.BtlCore.AppSystem.Caching
{
    public interface ICachingAppService : IApplicationService
    {
        Task ClearAllCaches();

        Task ClearCache(EntityDto<string> input);

        ListResultDto<CacheDto> GetAllCaches();
    }
}