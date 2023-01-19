using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace Wimi.BtlCore.Authorization.Users
{
    public class UserLinkManager : BtlCoreDomainServiceBase, IUserLinkManager
    {
        private readonly IRepository<UserAccount, long> userAccountRepository;

        public UserLinkManager(IRepository<UserAccount, long> userAccountRepository)
        {
            this.userAccountRepository = userAccountRepository;
        }

        [UnitOfWork]
        public virtual async Task<bool> AreUsersLinked(
            UserIdentifier firstUserIdentifier,
            UserIdentifier secondUserIdentifier)
        {
            var firstUserAccount = await this.GetUserAccountAsync(firstUserIdentifier);
            var secondUserAccount = await this.GetUserAccountAsync(secondUserIdentifier);

            if (!firstUserAccount.UserLinkId.HasValue || !secondUserAccount.UserLinkId.HasValue)
            {
                return false;
            }

            return firstUserAccount.UserLinkId == secondUserAccount.UserLinkId;
        }

        [UnitOfWork]
        public virtual async Task<UserAccount> GetUserAccountAsync(UserIdentifier userIdentifier)
        {
            return await this.userAccountRepository.FirstOrDefaultAsync(ua => ua.UserId == userIdentifier.UserId);
        }

        [UnitOfWork]
        public virtual async Task Link(User firstUser, User secondUser)
        {
            var currentUserIdentifier = firstUser.ToUserIdentifier();
            var toLinkUserIdentifier = secondUser.ToUserIdentifier();

            var firstUserAccount = await this.GetUserAccountAsync(currentUserIdentifier);
            var secondUserAccount = await this.GetUserAccountAsync(toLinkUserIdentifier);

            var userLinkId = firstUserAccount.UserLinkId ?? firstUserAccount.Id;
            firstUserAccount.UserLinkId = userLinkId;

            var userAccountsToLink = secondUserAccount.UserLinkId.HasValue
                                         ? this.userAccountRepository.GetAllList(
                                             ua => ua.UserLinkId == secondUserAccount.UserLinkId.Value)
                                         : new List<UserAccount> { secondUserAccount };

            userAccountsToLink.ForEach(u => { u.UserLinkId = userLinkId; });

            await this.CurrentUnitOfWork.SaveChangesAsync();
        }

        [UnitOfWork]
        public virtual async Task Unlink(UserIdentifier userIdentifier)
        {
            var targetUserAccount = await this.GetUserAccountAsync(userIdentifier);
            targetUserAccount.UserLinkId = null;

            await this.CurrentUnitOfWork.SaveChangesAsync();
        }
    }
}
