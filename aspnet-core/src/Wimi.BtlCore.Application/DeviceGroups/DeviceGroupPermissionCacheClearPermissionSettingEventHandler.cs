namespace Wimi.BtlCore.DeviceGroups
{
    using Abp.Dependency;
    using Abp.Events.Bus.Entities;
    using Abp.Events.Bus.Handlers;
    using Abp.Runtime.Caching;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.Runtime.Caching;

    public class DeviceGroupPermissionCacheClearPermissionSettingEventHandler : ITransientDependency, 
                                                                                IEventHandler<EntityCreatedEventData<DeviceGroupPermissionSetting>>, 
                                                                                IEventHandler<EntityChangedEventData<DeviceGroupPermissionSetting>>, 
                                                                                IEventHandler<EntityDeletedEventData<DeviceGroupPermissionSetting>>
    {
        private readonly ICacheManager cacheManager;

        public DeviceGroupPermissionCacheClearPermissionSettingEventHandler(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public void HandleEvent(EntityCreatedEventData<DeviceGroupPermissionSetting> eventData)
        {
            this.Clear();
        }

        public void HandleEvent(EntityChangedEventData<DeviceGroupPermissionSetting> eventData)
        {
            this.Clear();
        }

        public void HandleEvent(EntityDeletedEventData<DeviceGroupPermissionSetting> eventData)
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