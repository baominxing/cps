using System.Linq;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Runtime.Caching;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.AppSystem.Caching.Dto;
using Wimi.BtlCore.Authorization;

namespace Wimi.BtlCore.AppSystem.Caching
{
    [AbpAuthorize(PermissionNames.Pages_Administration_Host_Maintenance)]
    public class CachingAppService : BtlCoreAppServiceBase, ICachingAppService
    {
        private readonly ICacheManager cacheManager;

        public CachingAppService(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public async Task ClearAllCaches()
        {
            var caches = this.cacheManager.GetAllCaches();
            foreach (var cache in caches)
            {
                await cache.ClearAsync();
            }
        }

        public async Task ClearCache(EntityDto<string> input)
        {
            var cache = this.cacheManager.GetCache(input.Id);
            await cache.ClearAsync();
        }

        [HttpPost]
        public ListResultDto<CacheDto> GetAllCaches()
        {
            var caches = this.cacheManager.GetAllCaches().Select(cache => new CacheDto { Name = cache.Name }).ToList();

            return new ListResultDto<CacheDto>(caches);
        }
    }
}