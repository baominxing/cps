using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization.Users.Dto;
using Wimi.BtlCore.Dto;
using System.Linq.Dynamic.Core;
using Wimi.BtlCore.MultiTenancy;
using Microsoft.AspNetCore.Mvc;

namespace Wimi.BtlCore.Authorization.Users
{
    [AbpAuthorize]
    public class UserLinkAppService : BtlCoreAppServiceBase, IUserLinkAppService
    {
        private readonly AbpLoginResultTypeHelper abpLoginResultTypeHelper;

        private readonly LogInManager logInManager;

        private readonly IRepository<Tenant> tenantRepository;

        private readonly IRepository<UserAccount, long> userAccountRepository;

        private readonly IUserLinkManager userLinkManager;

        public UserLinkAppService(
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            IUserLinkManager userLinkManager,
            IRepository<Tenant> tenantRepository,
            IRepository<UserAccount, long> userAccountRepository,
            LogInManager logInManager)
        {
            this.abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            this.userLinkManager = userLinkManager;
            this.tenantRepository = tenantRepository;
            this.userAccountRepository = userAccountRepository;
            this.logInManager = logInManager;
        }

        [HttpPost]
        public async Task<PagedResultDto<LinkedUserDto>> GetLinkedUsers(GetLinkedUsersInputDto input)
        {
            var currentUserAccount = await this.userLinkManager.GetUserAccountAsync(this.AbpSession.ToUserIdentifier());
            if (currentUserAccount == null)
            {
                return new DatatablesPagedResultOutput<LinkedUserDto>(0, new List<LinkedUserDto>());
            }

            var query = this.CreateLinkedUsersQuery(currentUserAccount, input.Sorting);

            query = query.Skip(input.Start).Take(input.Length);

            var totalCount = await query.CountAsync();
            var linkedUsers = await query.ToListAsync();

            return new DatatablesPagedResultOutput<LinkedUserDto>(totalCount, linkedUsers);
        }

        [DisableAuditing]
        [HttpPost]
        public async Task<ListResultDto<LinkedUserDto>> GetRecentlyUsedLinkedUsers()
        {
            var query = await this.CreateLinkedUsersQuery("Id DESC");

            if (query == null)
            {
                return null;
            }

            var recentlyUsedlinkedUsers = await query.Skip(0).Take(3).ToListAsync();

            return new ListResultDto<LinkedUserDto>(recentlyUsedlinkedUsers);
        }

        public async Task LinkToUser(LinkToUserInputDto input)
        {
            var loginResult =
                await this.logInManager.LoginAsync(input.UsernameOrEmailAddress, input.Password, input.TenancyName);

            if (loginResult.Result != AbpLoginResultType.Success)
            {
                throw this.abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                    loginResult.Result,
                    input.UsernameOrEmailAddress,
                    input.TenancyName);
            }

            if (this.AbpSession.IsUser(loginResult.User))
            {
                throw new UserFriendlyException(this.L("YouCannotLinkToSameAccount"));
            }

            if (loginResult.User.ShouldChangePasswordOnNextLogin)
            {
                throw new UserFriendlyException(this.L("ChangePasswordBeforeLinkToAnAccount"));
            }

            var current = await this.GetCurrentUserAsync();
            var toLinkUser = loginResult.User;
            await this.userLinkManager.Link(current, toLinkUser);
        }

        public async Task UnlinkUser(UnlinkUserInputDto input)
        {
            var currentUserAccount = await this.userLinkManager.GetUserAccountAsync(this.AbpSession.ToUserIdentifier());

            if (!currentUserAccount.UserLinkId.HasValue)
            {
                throw new ApplicationException(this.L("YouAreNotLinkedToAnyAccount"));
            }

            if (!await this.userLinkManager.AreUsersLinked(this.AbpSession.ToUserIdentifier(), input.ToUserIdentifier()))
            {
                return;
            }

            await this.userLinkManager.Unlink(input.ToUserIdentifier());
        }

        private async Task<IQueryable<LinkedUserDto>> CreateLinkedUsersQuery(string sorting)
        {
            var currentUserIdentifier = this.AbpSession.ToUserIdentifier();
            var currentUserAccount = await this.userLinkManager.GetUserAccountAsync(this.AbpSession.ToUserIdentifier());

            if (currentUserAccount == null)
            {
                return null;
            }

            return (from userAccount in this.userAccountRepository.GetAll()
                    join tenant in this.tenantRepository.GetAll() on userAccount.TenantId equals tenant.Id into
                        tenantJoined
                    from tenant in tenantJoined.DefaultIfEmpty()
                    where
                        (userAccount.TenantId != currentUserIdentifier.TenantId
                         || userAccount.UserId != currentUserIdentifier.UserId) && userAccount.UserLinkId.HasValue
                        && userAccount.UserLinkId == currentUserAccount.UserLinkId
                    select
                        new LinkedUserDto
                        {
                            Id = userAccount.UserId,
                            TenantId = userAccount.TenantId,
                            TenancyName = tenant == null ? "." : tenant.TenancyName,
                            Username = userAccount.UserName
                            //LastLoginTime = userAccount.LastLoginTime
                        }).OrderBy(sorting);
        }

        private IQueryable<LinkedUserDto> CreateLinkedUsersQuery(UserAccount currentUserAccount, string sorting)
        {
            var currentUserIdentifier = this.AbpSession.ToUserIdentifier();

            return (from userAccount in this.userAccountRepository.GetAll()
                    join tenant in this.tenantRepository.GetAll() on userAccount.TenantId equals tenant.Id into
                        tenantJoined
                    from tenant in tenantJoined.DefaultIfEmpty()
                    where
                        (userAccount.TenantId != currentUserIdentifier.TenantId
                         || userAccount.UserId != currentUserIdentifier.UserId) && userAccount.UserLinkId.HasValue
                        && userAccount.UserLinkId == currentUserAccount.UserLinkId
                    select
                        new LinkedUserDto
                        {
                            Id = userAccount.UserId,
                            TenantId = userAccount.TenantId,
                            TenancyName = tenant == null ? "." : tenant.TenancyName,
                            Username = userAccount.UserName
                            //LastLoginTime = userAccount.LastLoginTime
                        }).OrderBy(sorting);
        }
    }
}
