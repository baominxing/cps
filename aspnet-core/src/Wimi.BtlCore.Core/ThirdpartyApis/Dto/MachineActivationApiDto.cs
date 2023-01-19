using System.Collections.Generic;

namespace Wimi.BtlCore.ThirdpartyApis.Dto
{
    public class MachineActivationApiDto
    {
        public double Activation { get; set; }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public int SortSeq { get; set; }

        public MachineActivationApiDto()
        {
            this.CurrentShiftMachineActivationsApi=new List<MachineActivationApiDto>();
        }

        public List<MachineActivationApiDto> CurrentShiftMachineActivationsApi { get; set; }
    }

}
