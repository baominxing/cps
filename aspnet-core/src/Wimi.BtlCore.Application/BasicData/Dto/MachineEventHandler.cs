using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Abp.Threading;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Machines.Manager;

namespace Wimi.BtlCore.BasicData.Dto
{
    public class MachineEventHandler : IEventHandler<MachineEventData>, ITransientDependency
    {
        private readonly IMachineManager machineManager;

        public MachineEventHandler(IMachineManager machineManager)
        {
            this.machineManager = machineManager;
        }

        public void HandleEvent(MachineEventData eventData)
        {
            AsyncHelper.RunSync(() => this.machineManager.DeleteMachine(eventData.machine));
        }
    }
}
