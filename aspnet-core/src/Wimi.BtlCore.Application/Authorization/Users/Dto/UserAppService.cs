using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Notifications;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization.Dto;
using Wimi.BtlCore.Authorization.Roles;
using Wimi.BtlCore.Authorization.Users.Exporting;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Notifications;

namespace Wimi.BtlCore.Authorization.Users.Dto
{
    [AbpAuthorize(PermissionNames.Pages_Administration_Users)]
    public class UserAppService : BtlCoreAppServiceBase, IUserAppService
    {
        private readonly IAppNotifier appNotifier;

        private readonly INotificationSubscriptionManager notificationSubscriptionManager;

        private readonly RoleManager roleManager;

        private readonly IUserEmailer userEmailer;

        private readonly IRepository<Role> roleRepository;
        private readonly IUserListExcelExporter userListExcelExporter;

        public UserAppService(
            RoleManager roleManager,
            IUserEmailer userEmailer,
            IUserListExcelExporter userListExcelExporter,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IRepository<Role> roleRepository,
            IAppNotifier appNotifier)
        {
            this.roleManager = roleManager;
            this.userEmailer = userEmailer;
            this.userListExcelExporter = userListExcelExporter;
            this.notificationSubscriptionManager = notificationSubscriptionManager;
            this.appNotifier = appNotifier;
            this.roleRepository = roleRepository;
        }

        [System.Obsolete]
        public async Task CreateOrUpdateUser(CreateOrUpdateUserInputDto input)
        {
            if (input.User.Id.HasValue)
            {
                await this.UpdateUserAsync(input);
            }
            else
            {
                await this.CreateUserAsync(input);
            }
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Delete)]
        public async Task DeleteUser(EntityDto<long> input)
        {
            if (input.Id == this.AbpSession.GetUserId())
            {
                throw new UserFriendlyException(this.L("YouCanNotDeleteOwnAccount"));
            }

            var user = await this.UserManager.GetUserByIdAsync(input.Id);
            this.CheckErrors(await this.UserManager.DeleteAsync(user));
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Create, PermissionNames.Pages_Administration_Users_Edit)]
        [System.Obsolete]
        public async Task<GetUserForEditOutputDto> GetUserForEdit(NullableIdDto<long> input)
        {
            // Getting all available roles
            var userRoleDtos =
                await
                this.roleManager.Roles.OrderBy(r => r.DisplayName)
                    .Select(r => new UserRoleDto { RoleId = r.Id, RoleName = r.Name, RoleDisplayName = r.DisplayName })
                    .ToArrayAsync();

            var output = new GetUserForEditOutputDto { Roles = userRoleDtos };

            if (!input.Id.HasValue)
            {
                // Creating a new user
                output.User = new UserEditDto { IsActive = true, ShouldChangePasswordOnNextLogin = true };

                foreach (var defaultRole in await this.roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
                {
                    var defaultUserRole = userRoleDtos.FirstOrDefault(ur => ur.RoleName == defaultRole.Name);
                    if (defaultUserRole != null)
                    {
                        defaultUserRole.IsAssigned = true;
                    }
                }
            }
            else
            {
                // Editing an existing user
                var user = await this.UserManager.GetUserByIdAsync(input.Id.Value);

                output.User = user.MapTo<UserEditDto>();
                output.ProfilePictureId = user.ProfilePictureId;

                foreach (var userRoleDto in userRoleDtos)
                {
                    userRoleDto.IsAssigned = await this.UserManager.IsInRoleAsync(user, userRoleDto.RoleName);
                }
            }

            return output;
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Users_ChangePermissions)]
        public async Task<GetUserPermissionsForEditOutputDto> GetUserPermissionsForEdit(EntityDto<long> input)
        {
            var user = await this.UserManager.GetUserByIdAsync(input.Id);
            var permissions = this.PermissionManager.GetAllPermissions();
            var grantedPermissions = await this.UserManager.GetGrantedPermissionsAsync(user);

            return new GetUserPermissionsForEditOutputDto
            {
                Permissions =
                             ObjectMapper.Map<List<FlatPermissionDto>>(permissions)
                               .OrderBy(p => p.DisplayName)
                               .ToList(),
                GrantedPermissionNames =
                               grantedPermissions.Select(p => p.Name).ToList()
            };
        }

        [HttpPost]
        public async Task<DatatablesPagedResultOutput<UserListDto>> GetUsers(GetUsersInputDto input)
        {
            // we distinguish search context by filter source:
            // 1. via datatables search filter, encapsulated in PagedSortedAndFilteredInputDto.Search
            // 2. via input we extra added, could add search context into Filter
            var query = this.UserManager.Users.Include(u => u.Roles);

            var queryWithFilter =
                query.WhereIf(
                    !input.Search.Value.IsNullOrWhiteSpace(),
                    u =>
                    u.Name.Contains(input.Search.Value) 
                    || u.UserName.Contains(input.Search.Value) || u.EmailAddress.Contains(input.Search.Value)
                    || u.WeChatId.Contains(input.Search.Value))
                    .WhereIf(
                        !input.Filter.IsNullOrWhiteSpace(),
                        u =>
                        u.Name.Contains(input.Filter) 
                        || u.UserName.Contains(input.Filter) || u.EmailAddress.Contains(input.Filter)
                        || u.WeChatId.Contains(input.Filter));

            // table page information should display: 
            // 第 1 至 1 + filter result count 项结果，共 total result count 项
            var totalCount = await query.CountAsync();
            var userCount = await queryWithFilter.CountAsync();
            var users = await queryWithFilter.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);

            await this.FillRoleNames(userListDtos);

            return new DatatablesPagedResultOutput<UserListDto>(userCount, userListDtos, totalCount, input.Draw);
        }

        public async Task<FileDto> GetUsersToExcel()
        {
            var users = await this.UserManager.Users.Include(u => u.Roles).ToListAsync();
            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await this.FillRoleNames(userListDtos);

            return this.userListExcelExporter.ExportToFile(userListDtos);
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Users_ChangePermissions)]
        public async Task ResetUserSpecificPermissions(EntityDto<long> input)
        {
            var user = await this.UserManager.GetUserByIdAsync(input.Id);
            await this.UserManager.ResetAllPermissionsAsync(user);
        }

        public async Task UnlockUser(EntityDto<long> input)
        {
            var user = await this.UserManager.GetUserByIdAsync(input.Id);
            user.Unlock();
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Users_ChangePermissions)]
        public async Task UpdateUserPermissions(UpdateUserPermissionsInputDto input)
        {
            var user = await this.UserManager.GetUserByIdAsync(input.Id);
            var grantedPermissions =
                this.PermissionManager.GetPermissionsFromNamesByValidating(input.GrantedPermissionNames);
            await this.UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Create)]
        [System.Obsolete]
        protected virtual async Task CreateUserAsync(CreateOrUpdateUserInputDto input)
        {
            var user = input.User.MapTo<User>(); // Passwords is not mapped (see mapping configuration)
            user.TenantId = this.AbpSession.TenantId;

            // Set password
            if (!input.User.Password.IsNullOrEmpty())
            {
                var passwordValidator = this.UserManager.PasswordValidators.FirstOrDefault();
                if(passwordValidator != null)
                {
                   this.CheckErrors(await passwordValidator.ValidateAsync(UserManager, user, input.User.Password));
                }
            }
            else
            {
                input.User.Password = User.CreateRandomPassword();
            }

            user.Password = new PasswordHasher<User>().HashPassword(user,input.User.Password);
            user.ShouldChangePasswordOnNextLogin = input.User.ShouldChangePasswordOnNextLogin;

            // Assign roles
            user.Roles = new Collection<UserRole>();
            foreach (var roleName in input.AssignedRoleNames)
            {
                var role = await this.roleManager.GetRoleByNameAsync(roleName);
                user.Roles.Add(new UserRole { RoleId = role.Id, TenantId = this.AbpSession.TenantId });
            }
            user.Surname = "WIMI";
            user.EmailAddress = $"{user.UserName}@{user.UserName}.com";

            this.CheckErrors(await this.UserManager.CreateAsync(user));
            await this.CurrentUnitOfWork.SaveChangesAsync(); // To get new user's Id.

            // Notifications
            await
                this.notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await this.appNotifier.WelcomeToTheApplicationAsync(user);

            // Send activation email
            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await this.userEmailer.SendEmailActivationLinkAsync(user, input.User.Password);
            }
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Edit)]
        protected virtual async Task UpdateUserAsync(CreateOrUpdateUserInputDto input)
        {
            Debug.Assert(input.User.Id != null, "input.User.Id should be set.");

            var user = await this.UserManager.FindByIdAsync(input.User.Id.Value.ToString());
            input.User.EmailAddress = user.EmailAddress;
            input.User.Surname = user.Surname;
            
            // Update user properties
            /*   input.User.MapTo(user);*/ // Passwords is not mapped (see mapping configuration)
            ObjectMapper.Map(input.User, user);

            if (!input.User.Password.IsNullOrEmpty())
            {
                this.CheckErrors(await this.UserManager.ChangePasswordAsync(user, input.User.Password));
            }

            this.CheckErrors(await this.UserManager.UpdateAsync(user));

            // Update roles
            this.CheckErrors(await this.UserManager.SetRolesAsync(user, input.AssignedRoleNames));

            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await this.userEmailer.SendEmailActivationLinkAsync(user, input.User.Password);
            }
        }

        public  async Task ResetPassword(ResetPasswordInput input)
        {
            var user = await this.UserManager.FindByIdAsync(input.User.Id.Value.ToString());

            if (!input.User.Password.IsNullOrEmpty())
            {
                this.CheckErrors(await this.UserManager.ChangePasswordAsync(user, input.User.Password));
            }
        }

        private async Task FillRoleNames(List<UserListDto> userListDtos)
        {
            /* This method is optimized to fill role names to given list. */
            var distinctRoleIds =
                (from userListDto in userListDtos
                 from userListRoleDto in userListDto.Roles
                 select userListRoleDto.RoleId).Distinct();

            var roleNames = new Dictionary<int, string>();
            foreach (var roleId in distinctRoleIds)
            {
                var role = await roleRepository.FirstOrDefaultAsync(roleId);
                if(role!=null)
                {
                    roleNames[roleId] = role.DisplayName;
                }
                //var role = await this.roleManager.GetRoleByIdAsync(roleId);
               
            }

            foreach (var userListDto in userListDtos)
            {
                foreach (var userListRoleDto in userListDto.Roles)
                {
                    if(roleNames.ContainsKey(userListRoleDto.RoleId))
                    {
                        userListRoleDto.RoleName = roleNames[userListRoleDto.RoleId];
                    }
                    else
                    {
                        userListRoleDto.RoleName = string.Empty;
                    }
                }

                userListDto.Roles = userListDto.Roles.OrderBy(r => r.RoleName).ToList();
            }
        }
    }
}
