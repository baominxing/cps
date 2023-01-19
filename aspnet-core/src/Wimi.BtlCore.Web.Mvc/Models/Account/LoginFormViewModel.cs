using Abp.MultiTenancy;

namespace Wimi.BtlCore.Web.Models.Account
{
    public class LoginFormViewModel
    {
        public string SuccessMessage { get; set; }

        public string UserNameOrEmailAddress { get; set; }

        public bool IsSelfRegistrationEnabled { get; set; }

        public bool IsTenantSelfRegistrationEnabled { get; set; }

        public string TenancyName { get; set; }

        public string ReturnUrl { get; set; }

        public bool IsMultiTenancyEnabled { get; set; }

        public bool IsSelfRegistrationAllowed { get; set; }

        public MultiTenancySides MultiTenancySide { get; set; }

        public LanguagesViewModel LanguageViewModel { get; set; }
    }
}
