using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Reasons.ReasonFeedbackCalendar.Dtos;
using Wimi.BtlCore.Web.Models.Reasons.ReasonFeedback;
using WIMI.BTL.ReasonFeedbackCalendar;

namespace WIMI.BTL.Web.Controllers
{
    [AbpMvcAuthorize(PermissionNames.Pages_ReasonFeedback_Calendar)]
    public class FeedbackCalendarController : BtlCoreControllerBase
    {
        private readonly IReasonFeedbackCalendarAppService feedbackcalendarAppService;

        public FeedbackCalendarController(IReasonFeedbackCalendarAppService feedbackcalendarAppService)
        {
            this.feedbackcalendarAppService = feedbackcalendarAppService;
        }

        // GET: FeedbackCalendar
        public ActionResult Index()
        {
            return this.View("~/Views/Reasons/FeedbackCalendar/Index.cshtml");
        }

        public async Task<PartialViewResult> CreateOrUpdateModal(int? id)
        {
            var model = new FeedbackCalendarViewModel();

            if (id.HasValue)
            {
                var dto = await this.feedbackcalendarAppService.Get(new FeedbackCalendarInputDto() { Id = id.Value });

                if (dto != null)
                {
                    model.Dto = dto;
                    model.IsEditMode = true;
                }
            }

            return this.PartialView("~/Views/Reasons/FeedbackCalendar/CreateOrUpdateModal.cshtml", model);
        }

        public PartialViewResult SelectMachineModal()
        {
            return this.PartialView("~/Views/Reasons/FeedbackCalendar/_SelectMachineModal.cshtml");
        }
    }
}