using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore.Authentication.TwoFactor
{
    [Serializable]
    public class TwoFactorCodeCacheItem
    {
        public const string CacheName = "AppTwoFactorCodeCache";

        public string Code { get; set; }

        public TwoFactorCodeCacheItem()
        {

        }

        public TwoFactorCodeCacheItem(string code)
        {
            Code = code;
        }
    }
}
