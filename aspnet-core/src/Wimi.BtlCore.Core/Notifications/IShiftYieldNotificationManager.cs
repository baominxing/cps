using System.Collections.Generic;
using System.Threading.Tasks;

using Abp.Domain.Services;
using Wimi.BtlCore.BasicData.Capacities;

namespace Wimi.BtlCore.Notifications
{
    public interface IShiftYieldNotificationManager : IDomainService, IWeixinNotificationBaseManager
    {
        Task<IEnumerable<Capacity>> ListShiftYields();

        void SetShiftSolutionItemId(int shiftSolutionItemId);
    }
}