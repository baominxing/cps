namespace Wimi.BtlCore.Notifications
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWeixinNotificationBaseManager
    {
        Task BuildMessageData(string agentid);

        Task<IEnumerable<WeixinMessageDataDto>> Build(string agentid);
    }
}