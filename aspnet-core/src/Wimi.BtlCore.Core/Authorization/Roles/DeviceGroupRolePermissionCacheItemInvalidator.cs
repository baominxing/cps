namespace Wimi.BtlCore.Authorization.Roles
{
    using Abp.Authorization.Roles;
    using Abp.Dependency;
    using Abp.Events.Bus.Entities;
    using Abp.Events.Bus.Handlers;
    using Abp.Runtime.Caching;
    using Wimi.BtlCore.Runtime.Caching;

    public class DeviceGroupRolePermissionCacheItemInvalidator :
        IEventHandler<EntityChangedEventData<DeviceGroupRolePermissionSetting>>, 
        IEventHandler<EntityDeletedEventData<AbpRoleBase>>, 
        ITransientDependency
    {
        private readonly ICacheManager cacheManager;

        public DeviceGroupRolePermissionCacheItemInvalidator(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public void HandleEvent(EntityChangedEventData<DeviceGroupRolePermissionSetting> eventData)
        {
            var cacheKey = eventData.Entity.RoleId.ToString();
            this.cacheManager.GetDeviceGroupRolePermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityDeletedEventData<AbpRoleBase> eventData)
        {
            var cacheKey = eventData.Entity.Id.ToString();
            this.cacheManager.GetDeviceGroupRolePermissionCache().Remove(cacheKey);
        }
    }
}