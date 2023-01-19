using Abp.AspNetCore.Mvc.ViewComponents;

namespace Wimi.BtlCore.Web.Views
{
    public abstract class BtlCoreViewComponent : AbpViewComponent
    {
        protected BtlCoreViewComponent()
        {
            LocalizationSourceName = BtlCoreConsts.LocalizationSourceName;
        }
    }
}
