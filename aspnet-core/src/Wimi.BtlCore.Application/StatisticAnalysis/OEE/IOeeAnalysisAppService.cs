namespace Wimi.BtlCore.StatisticAnalysis.OEE
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Application.Services;
    using Wimi.BtlCore.OEE;
    using Wimi.BtlCore.StatisticAnalysis.OEE.Dto;

    public interface IOeeAnalysisAppService : IApplicationService
    {
        Task<OeeDto> ListMachineOEEChart(OeeAnalysis input);

        Task<OeeDetailDto> GetMachineOEEDetail(OeeAnalysis input);

        Task<IEnumerable<MachineAvailabilityDto>> ListMachineAvailability(OeeAnalysis input);

        Task<IEnumerable<QualityStatusDto>> ListQualityIndicators(OeeAnalysis input);

        Task<IEnumerable<MachinePerformanceIndicator>> ListPerformanceIndicators(OeeAnalysis input);

        Task<OeeDto> ListOeeDetailTendencyChart(OeeAnalysis input);

        Task<OeeDetailDailyItemDto> GetDetailDailyItem(OeeAnalysis input);
    }
}