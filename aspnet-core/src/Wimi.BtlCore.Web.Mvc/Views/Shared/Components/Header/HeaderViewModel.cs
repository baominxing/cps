using System.Collections.Generic;
using Abp.Localization;
using Wimi.BtlCore.AppSystem.Sessions.Dto;

namespace Wimi.BtlCore.Web.Views.Shared.Components.Header
{
    public class HeaderViewModel
    {
        public LanguageInfo CurrentLanguage { get; set; }

        public bool IsImpersonatedLogin { get; set; }

        public bool IsMultiTenancyEnabled { get; set; }

        public IReadOnlyList<LanguageInfo> Languages { get; set; }

        public GetCurrentLoginInformationsOutput LoginInformations { get; set; }

        public string GetShownLoginName()
        {
            if(LoginInformations.User == null)
            {
                return string.Empty;
            }

            var userName = "<span id=\"HeaderCurrentUserName\">" + this.LoginInformations.User.UserName + "</span>";

            if (!this.IsMultiTenancyEnabled)
            {
                return userName;
            }

            return this.LoginInformations.Tenant == null
                       ? ".\\" + userName
                       : this.LoginInformations.Tenant.TenancyName + "\\" + userName;
        }
    }
}
