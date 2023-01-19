using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;

namespace Wimi.BtlCore.Web.Views
{
    public abstract class BtlCoreRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected BtlCoreRazorPage()
        {
            LocalizationSourceName = BtlCoreConsts.LocalizationSourceName;
        }
    }
}
