namespace Wimi.BtlCore.Runtime.Caching
{
    using Abp.Runtime.Caching;
    using Wimi.BtlCore.Authorization.Roles;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.Machines;

    public static class CacheManagerExtensions
    {
        public static ITypedCache<string, DeviceGroupCacheItem> GetDeviceGroupCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, DeviceGroupCacheItem>(DeviceGroupCacheItem.CacheStoreName);
        }

        public static ITypedCache<string, DeviceGroupRolePermissionCacheItem> GetDeviceGroupRolePermissionCache(
            this ICacheManager cacheManager)
        {
            return
                cacheManager.GetCache<string, DeviceGroupRolePermissionCacheItem>(
                    DeviceGroupRolePermissionCacheItem.CacheStoreName);
        }

        public static ITypedCache<string, MachineCacheItem> GetMachineCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, MachineCacheItem>(MachineCacheItem.CacheStoreName);
        }
    }
}