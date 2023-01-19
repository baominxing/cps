using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Abp.Threading;
using Wimi.BtlCore.BasicData.Shifts;

namespace Wimi.BtlCore.Shifts.Dto
{
    public class ShiftSolutionEventHandler : IEventHandler<ShiftSolutionEventData>, ITransientDependency
    {
        private readonly IShiftSolutionManager shiftSolutionManager;

        public ShiftSolutionEventHandler(IShiftSolutionManager shiftSolutionManager)
        {
            this.shiftSolutionManager = shiftSolutionManager;
        }

        public void HandleEvent(ShiftSolutionEventData eventData)
        {
            AsyncHelper.RunSync(() => this.shiftSolutionManager.DeleteById(eventData.ShiftSolutionId));
        }
    }
}
