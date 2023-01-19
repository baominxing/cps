using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wimi.BtlCore.RDLCReport.Dto
{
    [AutoMap(typeof(ProcessPlanResultDto))]
    public class ProcessPlanResultCopyDto : ProcessPlanResultDto
    {
    }
}
