namespace Wimi.BtlCore.DeviceGroups
{
    using Abp.Dependency;
    using Abp.Events.Bus.Entities;
    using Abp.Events.Bus.Handlers;
    using Abp.Runtime.Caching;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.Runtime.Caching;

    public class DevicrGroupPermissionCacheClearDeviceGroupEventHandler :
        IEventHandler<EntityCreatedEventData<DeviceGroup>>, 
        IEventHandler<EntityChangedEventData<DeviceGroup>>, 
        IEventHandler<EntityDeletedEventData<DeviceGroup>>, 
        ITransientDependency
    {
        private readonly ICacheManager cacheManager;

        public DevicrGroupPermissionCacheClearDeviceGroupEventHandler(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public void HandleEvent(EntityCreatedEventData<DeviceGroup> eventData)
        {
            this.Clear();
        }

        public void HandleEvent(EntityChangedEventData<DeviceGroup> eventData)
        {
            this.Clear();
        }

        public void HandleEvent(EntityDeletedEventData<DeviceGroup> eventData)
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