namespace Wimi.BtlCore.Notifications
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Domain.Services;

    public interface IWeixinNotificationProviderManager : IDomainService
    {
        Task AddRecord(NotificationRecord record);

        void UpdateRecordStatus(string content);

        Task<IEnumerable<WeixinMessageDataDto>> ListMessageDataDto(string agentid);

        Task<IEnumerable<WeixinMessageDataDto>> ListShiftYieldData(string agentid, int shiftSolutionItemId);
    }
}