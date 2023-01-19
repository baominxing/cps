using System.Collections.Generic;
using Wimi.BtlCore.ThirdpartyApis.Dto;

namespace Wimi.BtlCore.MultiTenancy.Dashboard.Dto
{
    public class GetMachineActivationDto
    {
        public GetMachineActivationDto()
        {
            this.CurrentShiftMachineActivations = new List<MachineActivationDto>();
            this.PreviousShiftMachineActivations = new List<MachineActivationDto>();
            this.CurrentShiftMachineActivationsApi=new List<MachineActivationApiDto>();
        }

        public List<MachineActivationApiDto> CurrentShiftMachineActivationsApi { get; set; }

        public List<MachineActivationDto> CurrentShiftMachineActivations { get; set; }

        public List<MachineActivationDto> PreviousShiftMachineActivations { get; set; }
    }
}