using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Traceability.Dto
{
    public class FlowSettingsDetailDto
    {
        public TraceFlowSettingDto FlowSetting { get; set; }

        public List<TraceRelatedMachineDto> RelatedMachines { get; set; }
    }
}
