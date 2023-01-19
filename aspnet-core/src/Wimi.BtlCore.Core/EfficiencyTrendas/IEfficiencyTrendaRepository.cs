using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Wimi.BtlCore.CommonEnums;
using Wimi.BtlCore.EfficiencyTrendas.Dtos;

namespace Wimi.BtlCore.EfficiencyTrendas
{
    public interface IEfficiencyTrendaRepository : ITransientDependency
    {
        List<EfficiencyTrendasDataTablesDto> GetEfficiencyTrendasDataTablesColumns(EfficiencyTrendsInputDto input);

        Task GetCurrentStateDurationStatistics(string machineIdList);

        Task<List<ExpandoObject>> GetEfficiencyTrendasExpandoObject(EnumStatisticalWays enmuStatisticalWays, List<int> machineId,
            List<int> machineShiftDetailId, List<string> machineShiftSolutionNameList, string queryType, DateTime startTime, DateTime endTime, List<string> unionTables);
    }
}
