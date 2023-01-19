namespace Wimi.BtlCore.Weixin
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Application.Services;
    using Abp.Application.Services.Dto;
    using Wimi.BtlCore.Notifications;
    using Wimi.BtlCore.Weixin.Dto;

    public interface IWeixinAppService : IApplicationService
    {
        Task AddMemberListToNotificationType(MemberListToNotificationTypeInputDto input);

        Task<NotificationTypeDto> CreateNotificationType(CreateNotificationTypeInputDto input);

        Task DeleteNotificationType(EntityDto input);

        Task<PagedResultDto<UserOutputDto>> FindUsers(FindUserInputDto input);

        Task<ListResultDto<NotificationTypeDto>> GetNotificationTypes();

        Task<PagedResultDto<NotificationTypeUserDto>> GetNotificationTypeUsers(GetNotificationTypeUsersInputDto input);

        Task RemoveMemberFromNotificationType(MemberToNotificationTypeInputDto input);

        string GetToken();

        void Send(WeixinMessageInputDto input);

        Task<IEnumerable<WeixinMessageDataDto>> ListWaitingMessageDatas();

        Task<IEnumerable<WeixinMessageDataDto>> ListShiftYieldDatas(int shiftSolutionItemId);

        Task<NotificationTypeDto> UpdateNotificationType(UpdateNotificationTypeInputDto input);

        Task UpdateUserIsActive(EntityDto input);
    }
}