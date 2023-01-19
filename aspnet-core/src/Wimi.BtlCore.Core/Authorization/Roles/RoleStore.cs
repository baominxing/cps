using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.DeviceGroups;

namespace Wimi.BtlCore.Authorization.Roles
{
    public class RoleStore : AbpRoleStore<Role, User>
    {
        private readonly IRepository<DeviceGroup> deviceGroupRepository;

        private readonly IRepository<DeviceGroupRolePermissionSetting, long> deviceGroupRolePermissionSettingRepository;

        public RoleStore(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Role> roleRepository,
            IRepository<RolePermissionSetting, long> rolePermissionSettingRepository,
            IRepository<DeviceGroup> deviceGroupRepository,
            IRepository<DeviceGroupRolePermissionSetting, long> deviceGroupRolePermissionSettingRepository)
            : base(
                unitOfWorkManager,
                roleRepository,
                rolePermissionSettingRepository)
        {
            this.deviceGroupRolePermissionSettingRepository = deviceGroupRolePermissionSettingRepository;
            this.deviceGroupRepository = deviceGroupRepository;
        }


        public async Task AddDeviceGroupPermissionAsync(
            Role role,
            DeviceGroupPermissionGrantInfo deviceGroupPermissionGrant)
        {
            if (await this.HasPermissionAsync(role.Id, deviceGroupPermissionGrant))
            {
                return;
            }

            await
                this.deviceGroupRolePermissionSettingRepository.InsertAsync(
                    new DeviceGroupRolePermissionSetting
                    {
                        TenantId = role.TenantId,
                        RoleId = role.Id,
                        DeviceGroupId = deviceGroupPermissionGrant.DeviceGroupId,
                        IsGranted = deviceGroupPermissionGrant.IsGranted
                    });
        }

        public async Task<IList<DeviceGroupPermissionGrantInfo>> GetDeviceGroupPermissionsAsync(int roleId)
        {
            var permission =
                (await this.deviceGroupRolePermissionSettingRepository.GetAllListAsync(p => p.RoleId == roleId))
                .Select(p => new DeviceGroupPermissionGrantInfo(p.DeviceGroupId, p.IsGranted))
                .ToList();

            return
                (await this.deviceGroupRepository.GetAllListAsync()).Select(
                    b => new DeviceGroupPermissionGrantInfo(b.Id, permission.Any(p => p.DeviceGroupId == b.Id)))
                    .ToList();
        }

        public virtual async Task<bool> HasPermissionAsync(
            int roleId,
            DeviceGroupPermissionGrantInfo deviceGroupPermissionGrant)
        {
            return
                await
                this.deviceGroupRolePermissionSettingRepository.CountAsync(
                    p =>
                    p.RoleId == roleId && p.DeviceGroupId == deviceGroupPermissionGrant.DeviceGroupId
                    && p.IsGranted == deviceGroupPermissionGrant.IsGranted) > 0;
        }

        /// <inheritdoc />
        public async Task RemoveDeviceGroupPermissionAsync(
            Role role,
            DeviceGroupPermissionGrantInfo deviceGroupPermissionGrant)
        {
            await
                this.deviceGroupRolePermissionSettingRepository.DeleteAsync(
                    permissionSetting =>
                    permissionSetting.RoleId == role.Id
                    && permissionSetting.DeviceGroupId == deviceGroupPermissionGrant.DeviceGroupId
                    && permissionSetting.IsGranted == deviceGroupPermissionGrant.IsGranted);
        }

        public async Task RemoveDeviceGroupPermissionAsync(Role role)
        {
            await
                this.deviceGroupRolePermissionSettingRepository.DeleteAsync(
                    permissionSetting => permissionSetting.RoleId == role.Id);
        }
    }
}
