using Abp;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.Collections.Generic;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.Notifications.Dto;
using Wimi.BtlCore.Web.Models.Common;

namespace Wimi.BtlCore.Web.Models.Notification.NotificationRules
{
    [AutoMap(typeof(GetNotificationRuleDto))]
    public class NotificationRuleModel : FullAuditedEntityDto, IDeviceGroupAndMachineWithPermissionsViewModal
    {
        public string DeviceGroupIds { get; set; }

        public string Name { get; set; }

        public int MessageType { get; set; }

        public List<NameValue<int>> MessageTypes { get; set; } = new List<NameValue<int>>();

        public bool IsEditMode { get; set; } = false;

        public List<FlatDeviceGroupDto> DeviceGroups { get; set; } = new List<FlatDeviceGroupDto>();

        public List<int> GrantedGroupIds { get; set; } = new List<int>();

        public List<FlatMachineDto> Machines { get; set; } = new List<FlatMachineDto>();
    }
}
