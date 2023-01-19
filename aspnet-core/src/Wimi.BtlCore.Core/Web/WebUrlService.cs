using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using System.Collections.Generic;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.Web
{
    public class WebUrlService : IWebUrlService, ITransientDependency
    {
        public const string TenancyNamePlaceHolder = "{TENANCY_NAME}";

        private readonly ISettingManager settingManager;

        public WebUrlService(ISettingManager settingManager)
        {
            this.settingManager = settingManager;
        }

        public string WebSiteRootAddressFormat { get; set; } = string.Empty;

        public string ServerRootAddressFormat { get; set; } = string.Empty;

        public bool SupportsTenancyNameInUrl { get; set; } = false;

        public List<string> GetRedirectAllowedExternalWebSites()
        {
            throw new System.NotImplementedException();
        }

        public string GetServerRootAddress(string tenancyName = null)
        {
            var siteRootFormat = this.settingManager.GetSettingValue(AppSettings.General.WebSiteRootAddress).EnsureEndsWith('/');

            if (!siteRootFormat.Contains(TenancyNamePlaceHolder))
            {
                return siteRootFormat;
            }

            if (siteRootFormat.Contains(TenancyNamePlaceHolder + "."))
            {
                siteRootFormat = siteRootFormat.Replace(TenancyNamePlaceHolder + ".", TenancyNamePlaceHolder);
            }

            if (tenancyName.IsNullOrEmpty())
            {
                return siteRootFormat.Replace(TenancyNamePlaceHolder, string.Empty);
            }

            return siteRootFormat.Replace(TenancyNamePlaceHolder, tenancyName + ".");
        }

        public string GetSiteRootAddress(string tenancyName = null)
        {
            var siteRootFormat = this.settingManager.GetSettingValue(AppSettings.General.WebSiteRootAddress).EnsureEndsWith('/');

            if (!siteRootFormat.Contains(TenancyNamePlaceHolder))
            {
                return siteRootFormat;
            }

            if (siteRootFormat.Contains(TenancyNamePlaceHolder + "."))
            {
                siteRootFormat = siteRootFormat.Replace(TenancyNamePlaceHolder + ".", TenancyNamePlaceHolder);
            }

            if (tenancyName.IsNullOrEmpty())
            {
                return siteRootFormat.Replace(TenancyNamePlaceHolder, string.Empty);
            }

            return siteRootFormat.Replace(TenancyNamePlaceHolder, tenancyName + ".");
        }
    }
}
