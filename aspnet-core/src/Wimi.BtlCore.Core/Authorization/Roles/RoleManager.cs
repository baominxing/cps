using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Zero.Configuration;
using Wimi.BtlCore.Authorization.Users;
using System.Threading;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Wimi.BtlCore.Runtime.Caching;
using Abp.Authorization.Users;

namespace Wimi.BtlCore.Authorization.Roles
{
    public class RoleManager : AbpRoleManager<Role, User>
    {
        private readonly ICacheManager cacheManager;

        private readonly RoleStore roleStore;

        private readonly IRepository<UserRole, long> userRoleRepository;

        private readonly IRepository<Role> roleRepository;

        public RoleManager(
            RoleStore store,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<AbpRoleManager<Role, User>> logger,
            IPermissionManager permissionManager,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRoleManagementConfig roleManagementConfig,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository)
            : base(
                  store,
                  roleValidators,
                  keyNormalizer,
                  errors, logger,
                  permissionManager,
                  cacheManager,
                  unitOfWorkManager,
                  roleManagementConfig,
                organizationUnitRepository,
                organizationUnitRoleRepository)
        {
            this.cacheManager = cacheManager;
            this.roleStore = store;
            this.userRoleRepository = userRoleRepository;
            this.roleRepository = roleRepository;
        }

        public List<string> GetCurrentUserRoles()
        {
            var result = this.userRoleRepository.GetAll()
               .Join(this.roleRepository.GetAll(), ur => ur.RoleId, r => r.Id, (ur, r) => new { UserRole = ur, Role = r })
               .Where(x => x.UserRole.UserId.Equals(AbpSession.UserId) && x.UserRole.TenantId == AbpSession.TenantId)
               .Select(x => x.Role.Name).ToList();

            return result;

            //var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            //if (claimsPrincipal == null)
            //{
            //    return null;
            //}

            //var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
            //if (claimsIdentity == null)
            //{
            //    return null;
            //}

            //return claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(q => q.Value).ToList();

            //var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            //if (claimsPrincipal == null)
            //{
            //    return null;
            //}

            //var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
            //if (claimsIdentity == null)
            //{
            //    return null;
            //}

            //return claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(q => q.Value).ToList();
        }

        public virtual async Task<DeviceGroupRolePermissionCacheItem> GetDeviceGroupRolePermission(int roleId)
        {
            // Get cached role permissions
            return await this.GetDeviceGroupRolePermissionCacheItemAsync(roleId);
        }

        public virtual async Task<DeviceGroupRolePermissionCacheItem> GetDeviceGroupRolePermission(Role role)
        {
            // Get cached role permissions
            return await this.GetDeviceGroupRolePermissionCacheItemAsync(role.Id);
        }

        public virtual async Task<IReadOnlyList<int>> GetGrantedDeviceGroupPermissionsAsync(int roleId)
        {
            // Get cached role permissions
            var cacheItem = await this.GetDeviceGroupRolePermission(roleId);
            return cacheItem.GrantedPermissions.ToList();
        }

        public virtual async Task<IReadOnlyList<int>> GetGrantedDeviceGroupPermissionsAsync(string roleName)
        {
            // Get cached role permissions
            var cacheItem = await this.GetDeviceGroupRolePermission(await this.GetRoleByNameAsync(roleName));
            return cacheItem.GrantedPermissions.ToList();
        }

        /// <summary>
        ///     get granted device groups of current user
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual async Task<IReadOnlyList<int>> GetGrantedDeviceGroupPermissionsAsync()
        {
            var roleNames = this.GetCurrentUserRoles();
            var grantedDeviceGroups = new List<int>();
            foreach (var roleName in roleNames)
            {
                var cacheItem = await this.GetDeviceGroupRolePermission(await this.GetRoleByNameAsync(roleName));
                grantedDeviceGroups.AddRange(cacheItem.GrantedPermissions);
            }

            // Get cached role permissions
            return grantedDeviceGroups;
        }

        /// <summary>
        /// Grants a device group permission for a role.
        /// </summary>
        /// <param name="role">
        /// Role
        /// </param>
        /// <param name="deviceGroupId">
        /// device group
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GrantDeviceGroupPermissionAsync(Role role, int deviceGroupId)
        {
            if (await this.IsDeviceGroupGrantedAsync(role.Id, deviceGroupId))
            {
                return;
            }

            await
                this.roleStore.AddDeviceGroupPermissionAsync(
                    role,
                    new DeviceGroupPermissionGrantInfo(deviceGroupId, true));
        }

        /// <summary>
        ///     Checks if a role is granted for a permission.
        /// </summary>
        /// <param name="roleId">role id</param>
        /// <param name="deviceGroupId">id of device group</param>
        /// <returns>True, if the role has the device group permission</returns>
        public virtual async Task<bool> IsDeviceGroupGrantedAsync(int roleId, int deviceGroupId)
        {
            // Get cached role permissions
            var cacheItem = await this.GetDeviceGroupRolePermissionCacheItemAsync(roleId);

            // Check the permission
            return cacheItem.GrantedPermissions.Contains(deviceGroupId);
        }

        /// <summary>
        /// Prohibits a device group permission for a role.
        /// </summary>
        /// <param name="role">
        /// Role
        /// </param>
        /// <param name="deviceGroupId">
        /// device group
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ProhibitDeviceGroupPermissionAsync(Role role, int deviceGroupId)
        {
            if (!await this.IsDeviceGroupGrantedAsync(role.Id, deviceGroupId))
            {
                return;
            }

            await
                this.roleStore.RemoveDeviceGroupPermissionAsync(
                    role,
                    new DeviceGroupPermissionGrantInfo(deviceGroupId, true));
        }

        public virtual async Task RemoveDeviceGroupPermissionAsync(Role role)
        {
            await this.roleStore.RemoveDeviceGroupPermissionAsync(role);
        }

        /// <summary>
        /// Sets all granted permissions of a role at once.
        ///     Prohibits all other permissions.
        /// </summary>
        /// <param name="roleId">
        /// Role id
        /// </param>
        /// <param name="permissions">
        /// Permissions
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task SetGrantedDeviceGroupPermissionsAsync(int roleId, IEnumerable<int> permissions)
        {
            await this.SetGrantedDeviceGroupPermissionsAsync(await this.GetRoleByIdAsync(roleId), permissions);
        }

        /// <summary>
        /// Sets all granted permissions of a role at once.
        ///     Prohibits all other permissions.
        /// </summary>
        /// <param name="role">
        /// The role
        /// </param>
        /// <param name="groupIds">
        /// ids of device groups
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task SetGrantedDeviceGroupPermissionsAsync(Role role, IEnumerable<int> groupIds)
        {
            var oldPermissions = await this.GetGrantedDeviceGroupPermissionsAsync(role.Id);
            var newPermissions = groupIds.ToArray();

            foreach (var permission in oldPermissions.Where(p => !newPermissions.Contains(p)))
            {
                await this.ProhibitDeviceGroupPermissionAsync(role, permission);
            }

            foreach (var permission in newPermissions.Where(p => !oldPermissions.Contains(p)))
            {
                await this.GrantDeviceGroupPermissionAsync(role, permission);
            }
        }

        private async Task<DeviceGroupRolePermissionCacheItem> GetDeviceGroupRolePermissionCacheItemAsync(int roleId)
        {
            var cacheKey = roleId.ToString();
            return await this.cacheManager.GetDeviceGroupRolePermissionCache().GetAsync(
                cacheKey,
                async () =>
                {
                    var newCacheItem = new DeviceGroupRolePermissionCacheItem(roleId);

                    foreach (var permissionInfo in await this.roleStore.GetDeviceGroupPermissionsAsync(roleId))
                    {
                        if (permissionInfo.IsGranted)
                        {
                            newCacheItem.GrantedPermissions.Add(permissionInfo.DeviceGroupId);
                        }
                        else
                        {
                            newCacheItem.ProhibitedPermissions.Add(permissionInfo.DeviceGroupId);
                        }
                    }

                    return newCacheItem;
                });
        }
    }
}
