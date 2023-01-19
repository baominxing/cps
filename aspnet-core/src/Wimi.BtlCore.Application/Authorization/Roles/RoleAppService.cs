namespace Wimi.BtlCore.Authorization.Roles
{
    using Abp.Application.Services.Dto;
    using Abp.Authorization;
    using Abp.Domain.Repositories;
    using Abp.UI;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Wimi.BtlCore;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.Authorization.Dto;
    using Wimi.BtlCore.Authorization.Roles.Dto;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.DeviceGroups.Dto;

    /// <summary>
    ///     Application service that is used by 'role management' page.
    /// </summary>
    [AbpAuthorize(PermissionNames.Pages_Administration_Roles)]
    public class RoleAppService : BtlCoreAppServiceBase, IRoleAppService
    {
        private readonly IRepository<DeviceGroup> deviceGroupRepository;

        private readonly RoleManager roleManager;

        public RoleAppService(
            RoleManager roleManager,
            IRepository<DeviceGroup> deviceGroupRepository)
        {
            this.roleManager = roleManager;
            this.deviceGroupRepository = deviceGroupRepository;
        }

        public async Task CreateOrUpdateRole(CreateOrUpdateRoleInputDto input)
        {
            if (input.Role.DisplayName != " ")
            {
                if (input.Role.Id.HasValue)
                {
                    await this.UpdateRoleAsync(input);
                }
                else
                {
                    await this.CreateRoleAsync(input);
                }
            }
            else
            {
                throw new UserFriendlyException(this.L("RoleNameNotBePureSpaces"));
            }
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Roles_Delete)]
        public async Task DeleteRole(EntityDto input)
        {
            var role = await this.roleManager.GetRoleByIdAsync(input.Id);
            this.CheckErrors(await this.roleManager.DeleteAsync(role));
            await this.RemoveDeviceGroupPermissionAsync(role);
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Roles_Create, PermissionNames.Pages_Administration_Roles_Edit)]
        [HttpPost]
        public async Task<GetRoleForEditOutputDto> GetRoleForEdit(NullableIdDto input)
        {
            var permissions = this.PermissionManager.GetAllPermissions();
            var deviceGroups = await this.deviceGroupRepository.GetAllListAsync();
            var grantedPermissions = new Permission[0];
            RoleEditDto roleEditDto;
            var grantedDeviceGroupPermissions = new List<int>();

            if (input.Id.HasValue)
            {
                // Editing existing role?
                var role = await this.roleManager.GetRoleByIdAsync(input.Id.Value);
                grantedPermissions = (await this.roleManager.GetGrantedPermissionsAsync(role)).ToArray();
                roleEditDto = ObjectMapper.Map<RoleEditDto>(role);

                grantedDeviceGroupPermissions =
                    (await this.roleManager.GetGrantedDeviceGroupPermissionsAsync(role.Id)).ToList();
            }
            else
            {
                roleEditDto = new RoleEditDto();
            }

            return new GetRoleForEditOutputDto
            {
                Role = roleEditDto,
                Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions)
                               .OrderBy(p => p.DisplayName)
                               .ToList(),
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList(),
                DeviceGroups = ObjectMapper.Map<List<DeviceGroupDto>>(deviceGroups),
                GrantedDeviceGroupPermissions = grantedDeviceGroupPermissions
            };
        }

        [HttpPost]
        public async Task<ListResultDto<RoleListDto>> GetRoles(GetRolesInputDto input)
        {
            var roles = await this.roleManager.Roles.ToListAsync();
            return new ListResultDto<RoleListDto>(ObjectMapper.Map<List<RoleListDto>>(roles));
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Roles_Create)]
        protected virtual async Task CreateRoleAsync(CreateOrUpdateRoleInputDto input)
        {
            var role = new Role(this.AbpSession.TenantId, input.Role.DisplayName) { IsDefault = input.Role.IsDefault };
            role.Name = input.Role.DisplayName;
            this.CheckErrors(await this.roleManager.CreateAsync(role));
            await this.CurrentUnitOfWork.SaveChangesAsync(); // It's done to get Id of the role.

            await this.UpdateGrantedPermissionsAsync(role, input.GrantedPermissionNames);
            await this.UpdateGrantedDeviceGroupPermissionsAsync(role, input.GrantedDeviceGroupPermissions);

        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Roles_Edit)]
        protected virtual async Task UpdateRoleAsync(CreateOrUpdateRoleInputDto input)
        {
            Debug.Assert(input.Role.Id != null, "input.Role.Id should be set.");

            var role = await this.roleManager.GetRoleByIdAsync(input.Role.Id.Value);
            role.Name = input.Role.DisplayName;
            this.CheckErrors(await this.roleManager.UpdateAsync(role));
            await this.CurrentUnitOfWork.SaveChangesAsync();
            role.DisplayName = input.Role.DisplayName;
            role.IsDefault = input.Role.IsDefault;

            await this.UpdateGrantedPermissionsAsync(role, input.GrantedPermissionNames);
            await this.UpdateGrantedDeviceGroupPermissionsAsync(role, input.GrantedDeviceGroupPermissions);
        }

        private async Task RemoveDeviceGroupPermissionAsync(Role role)
        {
            await this.roleManager.RemoveDeviceGroupPermissionAsync(role);
        }

        private async Task UpdateGrantedDeviceGroupPermissionsAsync(Role role, List<int> grantedDeviceGroupIds)
        {
            await this.roleManager.SetGrantedDeviceGroupPermissionsAsync(role, grantedDeviceGroupIds);
        }

        private async Task UpdateGrantedPermissionsAsync(Role role, List<string> grantedPermissionNames)
        {
            var grantedPermissions = this.PermissionManager.GetPermissionsFromNamesByValidating(grantedPermissionNames);
            await this.roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
        }
    }
}
