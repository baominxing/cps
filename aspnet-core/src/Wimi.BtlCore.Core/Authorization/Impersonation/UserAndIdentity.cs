using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Wimi.BtlCore.Authorization.Users;

namespace Wimi.BtlCore.Authorization.Impersonation
{
    public class UserAndIdentity
    {
        public User User { get; set; }

        public ClaimsIdentity Identity { get; set; }

        public UserAndIdentity(User user, ClaimsIdentity identity)
        {
            User = user;
            Identity = identity;
        }
    }
}
