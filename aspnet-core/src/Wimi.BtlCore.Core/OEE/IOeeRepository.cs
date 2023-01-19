namespace Wimi.BtlCore.OEE
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Application.Services.Dto;
    using Abp.Dependency;
    using Wimi.BtlCore.CommonEnums;

    public interface IOeeRepository : ITransientDependency
    {
        Task<IEnumerable<OeeResponse>> ListMachineAvailability(OeeAnalysis input);

        Task<IEnumerable<OeeResponse>> ListQualityIndicators(OeeAnalysis input);

        Task<IEnumerable<OeeResponse>> ListPerformanceIndicators(OeeAnalysis input);

        Task<IEnumerable<NameValueDto>> ListSummaryDate(OeeAnalysis input);

        Task<IEnumerable<int>> ListMachineIdInGroup(IEnumerable<int> groupIds);

        Task<IEnumerable<UnplannedPause>> ListUnplannedPause(int machineId, DateTime? startTime, DateTime? endTime);

        Task<IEnumerable<string>> ListRevisedDate(EnumStatisticalWays type, DateTime startTime, DateTime endTime);

        Task<ShiftOeeResponse> ListShiftMachineOee(OeeAnalysis input);

        Task<IEnumerable<ShiftDateRange>> ListShiftDateTimeRange(OeeAnalysis input);

        Task<IEnumerable<OeeResponse>> ListQualityIndicatorsItemByProduct(OeeAnalysis input);

        Task<IEnumerable<OeeResponse>> ListPerformanceIndicatorsItemByProduct(OeeAnalysis input);

        Task<ShiftOeeResponse> ListShiftMachineOeeByProduct(OeeAnalysis input);
    }
}