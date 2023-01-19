
using Wimi.BtlCore.BasicData.Machines;

namespace Wimi.BtlCore.BasicData.Dto
{
    public class BatchSwitchDto
    {
        public int MachineId { get; set; }

        public long[] ParamIds { get; set; }

        public bool Value { get; set; }

        public EnumStateParamType Type { get; set; }
    }
}