namespace Wimi.BtlCore.BasicData.DeviceGroups
{
    using Abp.Dependency;
    using Abp.Events.Bus.Entities;
    using Abp.Events.Bus.Handlers;
    using Abp.Runtime.Caching;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.Runtime.Caching;

    public class DeviceGroupCacheItemInvalidator : IEventHandler<EntityChangingEventData<DeviceGroup>>, 
                                                   ITransientDependency
    {
        private readonly ICacheManager cacheManager;

        public DeviceGroupCacheItemInvalidator(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public void HandleEvent(EntityChangingEventData<DeviceGroup> eventData)
        {
            // need update self cache
            var cacheKey = DeviceGroupCacheItem.CacheStoreName;
            this.cacheManager.GetDeviceGroupCache().Remove(cacheKey);

            // need update group id in flat machine
            var machineCacheKey = MachineCacheItem.CacheStoreName;
            this.cacheManager.GetMachineCache().Remove(machineCacheKey);

            // if device group deleted, group won't show in page again, thus it no business with permission cache
        }
    }
}