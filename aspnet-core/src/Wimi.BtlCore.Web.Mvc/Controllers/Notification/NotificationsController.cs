using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Notifications;
using Wimi.BtlCore.Notifications.Dto;

namespace Wimi.BtlCore.Web.Controllers.Notification
{
    [AbpMvcAuthorize]
    public class NotificationsController : BtlCoreControllerBase
    {
        private readonly INotificationAppService notificationApp;

        public NotificationsController(INotificationAppService notificationApp)
        {
            this.notificationApp = notificationApp;
        }

        public async Task<JsonResult> GetUserNotifications(GetUserNotificationsInputDto input)
        {
            var result = await this.notificationApp.GetUserNotifications(input);
            return this.Json(result);
        }

        public ActionResult Index()
        {
            return this.View();
        }

        public async Task<PartialViewResult> SettingsModal()
        {
            var notificationSettings = await this.notificationApp.GetNotificationSettings();
            return this.PartialView("~/Views/Notification/Notifications/_SettingsModal.cshtml", notificationSettings);
        }
    }
}
