using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.AppSystem.Sessions;

namespace Wimi.BtlCore.Web.Views.Shared.Components.Footer
{
    public class FooterViewComponent: BtlCoreViewComponent
    {
        private readonly ISessionAppService sessionAppService;

        public FooterViewComponent(ISessionAppService sessionAppService)
        {
            this.sessionAppService = sessionAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var footerModel = new FooterViewModel
            {
                LoginInformations =
                                     await this.sessionAppService.GetCurrentLoginInformations()
            };

            return View("_Footer", footerModel);
        }
    }
}
