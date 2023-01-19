namespace Wimi.BtlCore.Web.Controllers.Reasons
{
    using System.Threading.Tasks;
    using Abp.AspNetCore.Mvc.Authorization;
    using Abp.Domain.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.BasicData.StateInfos;
    using Wimi.BtlCore.Common;
    using Wimi.BtlCore.Controllers;
    using Wimi.BtlCore.Web.Models.Reasons.ReasonFeedback;

    [AbpMvcAuthorize(PermissionNames.Pages_ReasonFeedback_Feedback)]
    public class ReasonFeedbackController : BtlCoreControllerBase
    {
        private readonly ICommonLookupAppService commonLookupAppService;

        private readonly IRepository<StateInfo> statusInfoRepository;

        public ReasonFeedbackController(
            IRepository<StateInfo> statusInfoRepository,
            ICommonLookupAppService commonLookupAppService)
        {
            this.statusInfoRepository = statusInfoRepository;
            this.commonLookupAppService = commonLookupAppService;
        }

        public async Task<ActionResult> Index()
        {
            var deviceGroupWithPermissions = await this.commonLookupAppService.GetDeviceGroupWithPermissions();

            var model = new MachineStateViewModel()
            {
                StateInfoList = await this.statusInfoRepository.GetAllListAsync(),
                DeviceGroups = deviceGroupWithPermissions.DeviceGroups,
                GrantedGroupIds = deviceGroupWithPermissions.GrantedGroupIds
            };

            return this.View("~/Views/Reasons/ReasonFeedback/Index.cshtml", model);
        }

        public PartialViewResult FeedbackModal()
        {
            return this.PartialView("~/Views/Reasons/ReasonFeedback/_FeedbackModal.cshtml");
        }

        public PartialViewResult FinishFeedbackModal()
        {
            return this.PartialView("~/Views/Reasons/ReasonFeedback/_FinishFeedbackModal.cshtml");

        }

        public PartialViewResult FeedbackHistoryModal()
        {

            return this.PartialView("~/Views/Reasons/ReasonFeedback/_FeedbackHistoryModal.cshtml");
        }

    }
}