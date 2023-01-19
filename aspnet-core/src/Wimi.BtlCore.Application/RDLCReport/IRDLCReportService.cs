using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.RDLCReport.Dto;

namespace Wimi.BtlCore.RDLCReport
{
    public interface IRDLCReportService : IApplicationService
    {
        IList<StateConsumeTimeDto> GetStateConsumeTimeReportData(ReportInputDto input);

        IList<MachineUtilizationRateDto> GetMachineUtilizationRateData(ReportInputDto input);

        Task<IList<PersonYieldDto>> GetPersonYieldReportData(ReportInputDto input);

        IList<PersonPerfomanceDto> GetPersonPerformanceReportData(ReportInputDto input);

        IList<ProcessPlanResultDto> ListProcessPlanYield(ProductPlanYieldInputDto input);

        Task<IEnumerable<dynamic>> GetShiftSolutionList();
    }
}