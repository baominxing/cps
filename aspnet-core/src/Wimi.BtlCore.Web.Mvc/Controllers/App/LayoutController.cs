using System.Diagnostics;
using System.Net;
using System.Text;
using Abp.Application.Navigation;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.AppSystem.Sessions;
using Wimi.BtlCore.Web.Startup;
using Wimi.BtlCore.Web.Views.Shared.Components.Footer;
using Wimi.BtlCore.Web.Views.Shared.Components.Header;
using Wimi.BtlCore.Web.Views.Shared.Components.LanguageSelection;
using Wimi.BtlCore.Web.Views.Shared.Components.Sidebar;
using Wimi.BtlCore.Utilitis;
using System.Net.Http;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Web.Controllers.App
{
    public class LayoutController : BtlCoreControllerBase
    {
        private readonly ILanguageManager languageManager;
        private readonly IMultiTenancyConfig multiTenancyConfig;
        private readonly ISessionAppService sessionAppService;
        private readonly IUserNavigationManager userNavigationManager;
        private readonly IHttpClientFactory _clientFactory;

        public LayoutController(
            ISessionAppService sessionAppService,
            IUserNavigationManager userNavigationManager,
            IMultiTenancyConfig multiTenancyConfig,
            IHttpClientFactory clientFactory,
            ILanguageManager languageManager)
        {
            this.sessionAppService = sessionAppService;
            this.userNavigationManager = userNavigationManager;
            this.multiTenancyConfig = multiTenancyConfig;
            this.languageManager = languageManager;
            this._clientFactory = clientFactory;
        }

        public PartialViewResult Footer()
        {
            var footerModel = new FooterViewModel
            {
                LoginInformations = AsyncHelper.RunSync(() => this.sessionAppService.GetCurrentLoginInformations())
            };

            return this.PartialView("_Footer", footerModel);
        }


        public async Task<ActionResult> GetSentinelLdkFeatures()
        {
            var url = @"http://localhost:1947/_int_/tab_feat.html?haspid=0&featureid=-1&vendorid=0&productid=0&filterfrom=1&filterto=20";
            try
            {
                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                return this.Content(responseBody);
            }
            catch
            {
                return new EmptyResult();
            }
        }

        public PartialViewResult LanguageSelection()
        {
            var model = new LanguageSelectionViewModel
            {
                CurrentLanguage = languageManager.CurrentLanguage,
                Languages = languageManager.GetLanguages()
            };

            return PartialView("_LanguageSelection", model);
        }

        public PartialViewResult Header()
        {
            var headerModel = new HeaderViewModel
            {
                LoginInformations = AsyncHelper.RunSync(() => this.sessionAppService.GetCurrentLoginInformations()),
                Languages = this.languageManager.GetLanguages(),
                CurrentLanguage = this.languageManager.CurrentLanguage,
                IsMultiTenancyEnabled = this.multiTenancyConfig.IsEnabled,
                IsImpersonatedLogin = this.AbpSession.ImpersonatorUserId.HasValue
            };
            return this.PartialView("_Header", headerModel);
        }

        public PartialViewResult Sidebar(string currentPageName = "")
        {
            var sidebarModel = new SidebarViewModel
            {
                Menu = AsyncHelper.RunSync(() => this.userNavigationManager.GetMenuAsync(BtlCoreNavigationProvider.MenuName, this.AbpSession.ToUserIdentifier())),
                CurrentPageName = currentPageName
            };

            return this.PartialView("_Sidebar", sidebarModel);
        }

        public void ToggleSidebarCollapse()
        {
           // this.HttpContext.Session.SetString("sidebar-collapse", this.HttpContext.Session.GetString("sidebar-collapse") == null ? "sidebar-collapse" : null);
        }
    }
}
