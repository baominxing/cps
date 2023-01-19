using System.Collections.Generic;
using System.Threading.Tasks;

using Abp.Application.Services;
using Wimi.BtlCore.Order.MachineReport.Dtos;

namespace Wimi.BtlCore.Order.MachineReport
{
    public interface IMachineReportAppService : IApplicationService
    {
        Task<ShiftDayTimeRange.ShiftDayTimeRange> GetShiftDayTimeRange(MachineReportDefectiveReasonRequestDto input);

        Task<IEnumerable<MachineReportDefectiveReasonDto>> ListHourlyYieldAnalysis(MachineReportDefectiveReasonRequestDto input);

        Task<IEnumerable<MachineShiftDefectiveAnalysisDto>> ListMachineDefective(MachineShiftDefectiveAnalysisRequestDto input);

        Task<IEnumerable<MachinePartsDefectiveRecordDto>> ListMachineDefectiveRecords(MachineDefectiveRecordDto input);

        Task FeedbackDefectiveReason(MachineDefectiveRecordInputDto input);
    }
}
