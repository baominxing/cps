using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore.Authorization.Accounts.Dto
{
    public class ImpersonateOutput
    {
        public string ImpersonationToken { get; set; }

        public string TenancyName { get; set; }
    }
}
