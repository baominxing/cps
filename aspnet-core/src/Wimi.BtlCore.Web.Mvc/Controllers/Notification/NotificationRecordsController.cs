using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;

namespace Wimi.BtlCore.Web.Controllers.Notification
{
    public class NotificationRecordsController : BtlCoreControllerBase
    {
        // GET: NotificationRecords
        public ActionResult Index()
        {
            return this.View("~/Views/Notification/NotificationRecords/Index.cshtml");
        }
    }
}
