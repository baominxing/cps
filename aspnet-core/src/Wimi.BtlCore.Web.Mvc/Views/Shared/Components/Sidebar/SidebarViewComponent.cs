using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Navigation;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Web.Startup;

namespace Wimi.BtlCore.Web.Views.Shared.Components.Sidebar
{
    public class SidebarViewComponent: BtlCoreViewComponent
    {
        private readonly IUserNavigationManager _userNavigationManager;

        public SidebarViewComponent(IUserNavigationManager userNavigationManager)
        {
            this._userNavigationManager = userNavigationManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(string currentPageName = "")
        {

            var sidebarModel = new SidebarViewModel
            {
                Menu = await this._userNavigationManager.GetMenuAsync(
                                                   BtlCoreNavigationProvider.MenuName,
                                                   this.AbpSession.ToUserIdentifier()),
                CurrentPageName = currentPageName
            };

            return this.View("_Sidebar", sidebarModel);
        }
    }
}
