namespace Wimi.BtlCore.BasicData.Machines
{
    using Abp.Events.Bus;

    public class MachineEventData : EventData
    {
        public MachineEventData(Machine machine)
        {
            this.machine = machine;
        }

        public Machine machine;

    }
}
