using Abp;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Notifications;
using Wimi.BtlCore.Notifications.Dto;
using Wimi.BtlCore.Web.Models.Notification.NotificationRules;

namespace Wimi.BtlCore.Web.Controllers.Notification
{
    public class NotificationRulesController : BtlCoreControllerBase
    {
        private readonly INotificationAppService notificationAppService;

        public NotificationRulesController(
            INotificationAppService notificationAppService,
            INotificationService notificationService)
        {
            this.notificationAppService = notificationAppService;
        }

        // GET: NotificationRules
        public ActionResult Index()
        {
            return this.View("~/Views/Notification/NotificationRules/Index.cshtml");
        }

        public async Task<PartialViewResult> CreateOrUpdateModalForRulesType(int? id)
        {
            var model = new NotificationRuleModel
            {
                IsEditMode = false
            };
            foreach (int stateValue in Enum.GetValues(typeof(EnumMessageType)))
            {
                model.MessageTypes.Add(new NameValue<int>()
                {
                    Name = L(Enum.GetName(typeof(EnumMessageType), stateValue)),
                    Value = stateValue
                });

            }
            //model.MessageTypes.AddRange(typeof(EnumMessageType).ToNameValueList<int>());

            if (id.HasValue)
            {
                var notificationRule =
                    await this.notificationAppService.GetNotificationRule(
                        new NotificationRuleInputDto() { Id = (int)id });

                //notificationRule.MapTo(model);
                ObjectMapper.Map(notificationRule, model);
                model.IsEditMode = true;
            }

            return this.PartialView("~/Views/Notification/NotificationRules/_CreateOrUpdateModalForRulesType.cshtml", model);
        }

        public async Task<PartialViewResult> CreateOrUpdateModalForRules(int? id, int notificationRuleId)
        {
            var notificationRule = await this.notificationAppService.GetNotificationRule(new NotificationRuleInputDto() { Id = notificationRuleId });

            var model = new NotificationRuleDetailModel();

            if (id.HasValue)
            {
                var notificationRuleDetail = await this.notificationAppService.GetNotificationRuleDetail(new NotificationRuleDetailInputDto() { Id = (int)id });

                ObjectMapper.Map(notificationRuleDetail, model);
                model.NoticeUserIds = notificationRuleDetail.NoticeUserIds;

                model.IsEditMode = true;
            }
            else
            {
                model.IsEditMode = false;
            }

            model.NotificationRuleId = notificationRuleId;
            model.TriggerType = notificationRule.TriggerType;
            model.MessageType = notificationRule.MessageType;

            if (model.TriggerType == EnumTriggerType.TriggerWithShift)
            {
                model.ShiftSolutionList = await this.notificationAppService.ListShiftSolution();

                var firstOrDefault = model.ShiftSolutionList.FirstOrDefault();

                if (firstOrDefault != null)
                {
                    model.ShiftList = await this.notificationAppService.ListShift(
                                          new NotificationRuleDetailInputDto()
                                          {
                                              Id = id ?? 0,
                                              ShiftSolutionId =
                                                      id.HasValue
                                                          ? model.ShiftSolutionId
                                                          : firstOrDefault.Value
                                          });
                }
            }

            return this.PartialView("~/Views/Notification/NotificationRules/_CreateOrUpdateModalForRules.cshtml", model);
        }
    }
}
