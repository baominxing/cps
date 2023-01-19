using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Models.Common.Modals;

namespace Wimi.BtlCore.Web.Controllers.App
{
    [AbpMvcAuthorize]
    public class CommonController : BtlCoreControllerBase
    {
        public PartialViewResult LookupModal(LookupModalViewModel model)
        {
            return this.PartialView("Modals/_LookupModal", model);
        }
    }
}
