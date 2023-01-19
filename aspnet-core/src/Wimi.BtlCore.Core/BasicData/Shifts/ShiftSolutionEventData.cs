using Abp.Events.Bus;

namespace Wimi.BtlCore.BasicData.Shifts
{
    public class ShiftSolutionEventData : EventData
    {
        public ShiftSolutionEventData(int shiftSolutionId)
        {
            this.ShiftSolutionId = shiftSolutionId;
        }
        public int ShiftSolutionId { get; set; }
    }
}
