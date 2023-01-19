namespace Wimi.BtlCore.Web.Controllers.BasicData
{
    using Abp.AspNetCore.Mvc.Authorization;
    using Abp.Domain.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.Controllers;
    using Wimi.BtlCore.Web.Models.BasicData.Weixin;
    using Wimi.BtlCore.WeChart;

    public class WeChatNotificationsController : BtlCoreControllerBase
    {
        private readonly IRepository<NotificationType> notificationTypeRepository;

        public WeChatNotificationsController(IRepository<NotificationType> notificationType)
        {
            this.notificationTypeRepository = notificationType;
        }

        protected WeChatNotificationsController()
        {
        }

        public PartialViewResult AddModal()
        {
            return this.PartialView("_AddModal");
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Administration_WeChatNotifications)]
        public PartialViewResult CreateModal()
        {
            return this.PartialView("_CreateModal");
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Administration_WeChatNotifications)]
        public async Task<PartialViewResult> EditModal(int id)
        {
            var notificationType = await this.notificationTypeRepository.GetAsync(id);
            var model = ObjectMapper.Map<EditNotificationTypeModalViewModel>(notificationType);

            return this.PartialView("_EditModal", model);
        }

        // GET: WeChatNotifications
        [AbpMvcAuthorize(PermissionNames.Pages_Administration_WeChatNotifications)]
        public ActionResult Index()
        {
            return this.View();
        }
    }
}