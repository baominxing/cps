namespace Wimi.BtlCore.BasicData.Machines
{
    using Abp.Dependency;
    using Abp.Events.Bus.Entities;
    using Abp.Events.Bus.Handlers;
    using Abp.Runtime.Caching;
    using Wimi.BtlCore.Runtime.Caching;

    public class MachineCacheItemInvalidator : IEventHandler<EntityChangedEventData<Machine>>, ITransientDependency
    {
        private readonly ICacheManager cacheManager;

        public MachineCacheItemInvalidator(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public void HandleEvent(EntityChangedEventData<Machine> eventData)
        {
            var cacheKey = MachineCacheItem.CacheStoreName;
            this.cacheManager.GetMachineCache().Remove(cacheKey);
        }
    }
}