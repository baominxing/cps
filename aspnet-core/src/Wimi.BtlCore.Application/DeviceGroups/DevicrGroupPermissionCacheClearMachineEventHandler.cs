namespace Wimi.BtlCore.DeviceGroups
{
    using Abp.Dependency;
    using Abp.Events.Bus.Entities;
    using Abp.Events.Bus.Handlers;
    using Abp.Runtime.Caching;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.Runtime.Caching;

    public class DevicrGroupPermissionCacheClearMachineEventHandler : IEventHandler<EntityCreatedEventData<Machine>>, 
                                                                      IEventHandler<EntityChangedEventData<Machine>>, 
                                                                      IEventHandler<EntityDeletedEventData<Machine>>, 
                                                                      ITransientDependency
    {
        private readonly ICacheManager cacheManager;

        public DevicrGroupPermissionCacheClearMachineEventHandler(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public void HandleEvent(EntityCreatedEventData<Machine> eventData)
        {
            this.Clear();
        }

        public void HandleEvent(EntityChangedEventData<Machine> eventData)
        {
            this.Clear();
        }

        public void HandleEvent(EntityDeletedEventData<Machine> eventData)
        {
            this.Clear();
        }

        private void Clear()
        {
            this.cacheManager.GetDeviceGroupRolePermissionCache().Clear();
            this.cacheManager.GetDeviceGroupCache().Clear();
            this.cacheManager.GetMachineCache().Clear();
        }
    }
}