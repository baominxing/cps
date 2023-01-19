namespace Wimi.BtlCore.Authorization.Roles
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class DeviceGroupRolePermissionCacheItem
    {
        public const string CacheStoreName = "DeviceGroupRolePermissionCacheItem";
        
        public DeviceGroupRolePermissionCacheItem()
        {
            this.GrantedPermissions = new HashSet<int>();
            this.ProhibitedPermissions = new HashSet<int>();
        }

        public DeviceGroupRolePermissionCacheItem(int roleId)
            : this()
        {
            this.RoleId = roleId;
        }

        public HashSet<int> GrantedPermissions { get; set; }

        public HashSet<int> ProhibitedPermissions { get; set; }

        public long RoleId { get; set; }
    }
}