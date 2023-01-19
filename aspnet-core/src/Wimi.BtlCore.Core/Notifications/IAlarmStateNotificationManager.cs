using System.Collections.Generic;
using System.Threading.Tasks;

using Abp.Domain.Services;
using Wimi.BtlCore.BasicData.States;
using Wimi.BtlCore.Machines;

namespace Wimi.BtlCore.Notifications
{
    public interface IAlarmStateNotificationManager : IWeixinNotificationBaseManager, IDomainService
    {
        Task<IEnumerable<State>> ListAlarmingStatesMachine();
    }
}