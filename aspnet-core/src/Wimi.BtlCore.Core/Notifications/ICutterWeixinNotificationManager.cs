namespace Wimi.BtlCore.Notifications
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Domain.Services;
    using Wimi.BtlCore.Cutter;

    public interface ICutterWeixinNotificationManager : IDomainService, IWeixinNotificationBaseManager
    {
        Task<IEnumerable<CutterStates>> ListWarningCutters();
    }
}