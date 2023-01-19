using Abp;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.Collections.Generic;
using Wimi.BtlCore.Notifications;
using Wimi.BtlCore.Notifications.Dto;

namespace Wimi.BtlCore.Web.Models.Notification.NotificationRules
{
    [AutoMap(typeof(GetNotificationRuleDetailDto))]
    public class NotificationRuleDetailModel : FullAuditedEntityDto
    {
        public int NotificationRuleId { get; set; }

        public int TriggerCondition { get; set; }

        public bool IsEnabled { get; set; }

        public string NoticeUserIds { get; set; }

        public string NoticeUserNames { get; set; }

        public bool IsEditMode { get; set; } = false;

        public int ShiftSolutionId { get; set; }

        public int ShiftId { get; set; }

        public string ShiftInfoName { get; set; }

        public IEnumerable<NameValue<int>> UserList { get; set; } = new List<NameValue<int>>();

        public IEnumerable<NameValue<int>> ShiftSolutionList { get; set; } = new List<NameValue<int>>();

        public IEnumerable<NameValue<int>> ShiftList { get; set; } = new List<NameValue<int>>();

        public EnumTriggerType TriggerType { get; set; }

        public EnumMessageType MessageType { get; set; }
    }
}
