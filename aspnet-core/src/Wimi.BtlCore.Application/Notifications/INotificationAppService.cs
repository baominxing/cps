namespace Wimi.BtlCore.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp;
    using Abp.Application.Services;
    using Abp.Application.Services.Dto;

    using Wimi.BtlCore.Notifications.Dto;

    public interface INotificationAppService : IApplicationService
    {
        Task<GetNotificationSettingsOutputDto> GetNotificationSettings();

        Task<GetNotificationsOutputDto> GetUserNotifications(GetUserNotificationsInputDto input);

        Task SetAllNotificationsAsRead();

        Task SetNotificationAsRead(EntityDto<Guid> input);

        Task UpdateNotificationSettings(UpdateNotificationSettingsInputDto input);

        Task<IEnumerable<GetNotificationRuleDto>> ListNotificationRule();

        Task<IEnumerable<int>> ListReferencedDeviceGroupId(NotificationRuleInputDto input);

        Task<GetNotificationRuleDto> GetNotificationRule(NotificationRuleInputDto input);

        Task<GetNotificationRuleDto> CreateNotificationRule(NotificationRuleInputDto input);

        Task<GetNotificationRuleDto> UpdateNotificationRule(NotificationRuleInputDto input);

        Task DeleteNotificationRule(NotificationRuleInputDto input);

        Task<IEnumerable<GetNotificationRuleDetailDto>> ListNotificationRuleDetail(NotificationRuleInputDto input);

        Task<GetNotificationRuleDetailDto> GetNotificationRuleDetail(NotificationRuleDetailInputDto input);

        Task<GetNotificationRuleDetailDto> CreateNotificationRuleDetail(NotificationRuleDetailInputDto input);

        Task<GetNotificationRuleDetailDto> UpdateNotificationRuleDetail(NotificationRuleDetailInputDto input);

        Task DeleteNotificationRuleDetail(NotificationRuleDetailInputDto input);

        Task<IEnumerable<GetNotificationRecordDto>> ListNotificationRecord(NotificationRuleDetailInputDto input);

        Task<IEnumerable<UserDto>> GetUserList();

        Task<IEnumerable<GetNotificationRuleDetailDto>> GetReferencedShiftIds();

        Task<IEnumerable<NameValue<int>>> ListShiftSolution();

        Task<IEnumerable<NameValue<int>>> ListShift(NotificationRuleDetailInputDto input);
    }
}