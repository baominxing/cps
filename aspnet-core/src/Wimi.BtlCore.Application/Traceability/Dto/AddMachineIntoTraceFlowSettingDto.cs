using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Traceability.Dto
{
    public class AddMachineIntoTraceFlowSettingDto
    {
        public int TraceFlowSettingId { get; set; }

        public TraceRelatedMachineDto RelatedMachineDto { get; set; }
    }
}
