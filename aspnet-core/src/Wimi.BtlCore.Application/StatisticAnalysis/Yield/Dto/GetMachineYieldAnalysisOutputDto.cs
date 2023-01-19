using System.Collections.Generic;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;

namespace Wimi.BtlCore.StatisticAnalysis.Yield.Dto
{
    public class GetMachineYieldAnalysisOutputDto
    {
        public IEnumerable<MachineYieldAnalysisOutputDto> MachineYieldAnalysisRecordList;

        public int TotalCount;
    }
}
