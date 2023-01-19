namespace Wimi.BtlCore.DeviceGroups
{
    using Abp.Dependency;
    using Abp.Events.Bus.Entities;
    using Abp.Events.Bus.Handlers;
    using Abp.Runtime.Caching;
    using Wimi.BtlCore.Authorization.Roles;
    using Wimi.BtlCore.Runtime.Caching;

    /// <summary>
    /// The device group permission cache clear device group role permission setting event handler.
    /// </summary>
    public class DeviceGroupPermissionCacheClearDeviceGroupRolePermissionSettingEventHandler : ITransientDependency, 
                                                                                               IEventHandler<EntityCreatedEventData<DeviceGroupRolePermissionSetting>>, 
                                                                                               IEventHandler<EntityChangedEventData<DeviceGroupRolePermissionSetting>>, 
                                                                                               IEventHandler<EntityDeletedEventData<DeviceGroupRolePermissionSetting>>
    {
        private readonly ICacheManager cacheManager;

        public DeviceGroupPermissionCacheClearDeviceGroupRolePermissionSettingEventHandler(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public void HandleEvent(EntityCreatedEventData<DeviceGroupRolePermissionSetting> eventData)
        {
            this.Clear();
        }

        public void HandleEvent(EntityChangedEventData<DeviceGroupRolePermissionSetting> eventData)
        {
            this.Clear();
        }

        public void HandleEvent(EntityDeletedEventData<DeviceGroupRolePermissionSetting> eventData)
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