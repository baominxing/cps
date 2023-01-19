using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.Authorization.Users;

namespace Wimi.BtlCore.Authorization.Users
{
    public interface IUserLinkManager
    {
        Task<bool> AreUsersLinked(UserIdentifier firstUserIdentifier, UserIdentifier secondUserIdentifier);

        Task<UserAccount> GetUserAccountAsync(UserIdentifier userIdentifier);

        Task Link(User firstUser, User secondUser);

        Task Unlink(UserIdentifier userIdentifier);
    }
}
