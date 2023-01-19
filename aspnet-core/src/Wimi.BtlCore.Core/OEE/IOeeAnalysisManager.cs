namespace Wimi.BtlCore.OEE
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Domain.Services;
    using Wimi.BtlCore.CommonEnums;

    public interface IOeeAnalysisManager :IDomainService
    {
        Task<IEnumerable<int>> GetFilterIds(EnumQueryMethod type, IEnumerable<int> machineIdList, bool batch = true);

        Task<IEnumerable<string>> ListRevisedDate(EnumStatisticalWays type, DateTime startTime, DateTime endTime);

        Task<OeeAnalysis> FormartInputDto(OeeAnalysis input);
    }
}