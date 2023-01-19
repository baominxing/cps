using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;
using System;
using System.Linq;

namespace Wimi.BtlCore.Web.Controllers.BasicData
{
    public class AppController : BtlCoreControllerBase
    {
        private const string CookiePageName = "page_name";

        public IActionResult Index()
        {
            if (this.Request.Cookies.ContainsKey(CookiePageName))
            {
                var value = this.Request.Cookies[CookiePageName];
                if (!string.IsNullOrEmpty(value))
                {
                    var pageName = value.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
                    ViewBag.CurrentPageName = $"App.{pageName}";
                    Logger.Debug($"路由解析成功,page_name=[{pageName}]");
                }
            }
            return View("/Views/App/VueApps/Index.cshtml");
        }
    }
}
