namespace Wimi.BtlCore.Authorization.Roles
{
    using Wimi.BtlCore.BasicData.DeviceGroups;

    public class DeviceGroupRolePermissionSetting : DeviceGroupPermissionSetting
    {
        /// <summary>
        ///     Role id.
        /// </summary>
        public virtual int RoleId { get; set; }
    }
}