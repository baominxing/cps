using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Wimi.BtlCore.Controllers
{
    public abstract class BtlCoreControllerBase: AbpController
    {
        protected BtlCoreControllerBase()
        {
            LocalizationSourceName = BtlCoreConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        protected virtual void CheckModelState()
        {
            if (!this.ModelState.IsValid)
            {
                throw new UserFriendlyException(this.L("FormIsNotValidMessage"));
            }
        }

        protected SelectList GetYesNoSelectListItems()
        {
            var list = new[]
                           {
                               new SelectListItem() { Text = this.L("Yes"), Value = "true" },
                               new SelectListItem() { Text = this.L("No"), Value = "false" }
                           };

            return new SelectList(list, "Value", "Text");
        }
    }
}
