using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers.App
{
    public class DmpMachineSettingController : BtlCoreControllerBase
    {
        public ActionResult Index()
        {
            return this.View("~/Views/App/DmpMachineSetting/Index.cshtml");
        }
    }
}
