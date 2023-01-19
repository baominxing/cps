namespace Wimi.BtlCore.DeviceGroups
{
    using Abp.Dependency;
    using Abp.Events.Bus.Entities;
    using Abp.Events.Bus.Handlers;
    using Abp.Runtime.Caching;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.Runtime.Caching;

    public class DeviceGroupPermissionCacheClearMachineDevicrGroupEventHandler : ITransientDependency, 
                                                                                 IEventHandler<EntityCreatedEventData<MachineDeviceGroup>>, 
                                                                                 IEventHandler<EntityChangedEventData<MachineDeviceGroup>>, 
                                                                                 IEventHandler<EntityDeletedEventData<MachineDeviceGroup>>
    {
        private readonly ICacheManager cacheManager;

        public DeviceGroupPermissionCacheClearMachineDevicrGroupEventHandler(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public void HandleEvent(EntityCreatedEventData<MachineDeviceGroup> eventData)
        {
            this.Clear();
        }

        public void HandleEvent(EntityChangedEventData<MachineDeviceGroup> eventData)
        {
            this.Clear();
        }

        public void HandleEvent(EntityDeletedEventData<MachineDeviceGroup> eventData)
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