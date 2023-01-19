using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.MultiTenancy;

namespace Wimi.BtlCore.Web.Startup
{
    public class TenancyNameFinder : ITenancyNameFinder, ITransientDependency
    {
        private readonly IMultiTenancyConfig multiTenancyConfig;

        private readonly ISettingManager settingManager;

        public TenancyNameFinder(
            ISettingManager settingManager, 
            IMultiTenancyConfig multiTenancyConfig)
        {
            this.settingManager = settingManager;
            this.multiTenancyConfig = multiTenancyConfig;
        }

        public string GetCurrentTenancyNameOrNull()
        {
            if (!this.multiTenancyConfig.IsEnabled)
            {
                return Tenant.DefaultTenantName;
            }

            var siteRootFormat = this.settingManager.GetSettingValue(AppSettings.General.WebSiteRootAddress).EnsureEndsWith('/');

            if (!siteRootFormat.Contains(WebUrlService.TenancyNamePlaceHolder))
            {
                // Web site does not support subdomain tenant name
                return null;
            }

            var currentRootAddress = GetCurrentSiteRootAddress().EnsureEndsWith('/');

            string[] values;
            if (!FormattedStringValueExtracter.IsMatch(currentRootAddress, siteRootFormat, out values, true))
            {
                return null;
            }

            return values.Length <= 0 ? null : values[0];
        }

        private static string GetCurrentSiteRootAddress()
        {

            //if (HttpContext.Current == null)
            //{
            //    // Can not find current URL
            //    return null;
            //}

            //var url = HttpContext.Current.Request.Url;

            //return url.Scheme + Uri.SchemeDelimiter + url.Host + (url.IsDefaultPort ? string.Empty : ":" + url.Port)
            //       + HttpContext.Current.Request.ApplicationPath;

            return string.Empty;
        }
    }
}
