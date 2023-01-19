using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.Localization;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.AppSystem.Sessions;

namespace Wimi.BtlCore.Web.Views.Shared.Components.Header
{
    public class HeaderViewComponent: BtlCoreViewComponent
    {
        private readonly ISessionAppService _sessionAppService;

        private readonly ILanguageManager _languageManager;

        private readonly IMultiTenancyConfig _multiTenancyConfig;

        public HeaderViewComponent(ISessionAppService sessionAppService, ILanguageManager languageManager, IMultiTenancyConfig multiTenancyConfig)
        {
            this._sessionAppService = sessionAppService;
            this._languageManager = languageManager;
            this._multiTenancyConfig = multiTenancyConfig;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var headerModel = new HeaderViewModel
            {
                LoginInformations = await
                                            this._sessionAppService.GetCurrentLoginInformations(),
                Languages = this._languageManager.GetLanguages(),
                CurrentLanguage = this._languageManager.CurrentLanguage,
                IsMultiTenancyEnabled = this._multiTenancyConfig.IsEnabled,
                IsImpersonatedLogin = this.AbpSession.ImpersonatorUserId.HasValue
            };

            return this.View("_Header", headerModel);
        }
    }
}
