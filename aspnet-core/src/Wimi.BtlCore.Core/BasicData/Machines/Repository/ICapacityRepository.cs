using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.CommonEnums;

namespace Wimi.BtlCore.BasicData.Capacities
{
    public interface ICapacityRepository : ITransientDependency
    {
        string GetMaxCapacitySyncDateTime();

        GetMachineStateRateInputDto GetStartTimeOfGanttChart(GetMachineStateRateInputDto input);
 
        Task<IEnumerable<MachineStateDto>> GetSummaryDate(GetMachineStateRateInputDto input);

        Task<IEnumerable<Yield4PerProgramOutputDto>> GetMachineAvgProgramDurationAndYield(int machineId,string startTime,string endTime);

        Task<MachineYieldDto> GetMachineCapability(EnumStatisticalWays statisticalWay, EnumQueryMethod queryMethod,
            List<int> machineIdList, List<int> shiftSolutionIdList, List<int> deviceGroupIdList, string startTime, string endTime, List<string> unionTables);
    }
}
