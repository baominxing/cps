using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.BasicData.Shifts.Manager.Dto;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.StatisticAnalysis.States.Dto;

namespace Wimi.BtlCore.StatisticAnalysis.States
{
    public interface IStatesAppService : IApplicationService
    {
        Task<IEnumerable<MachineStateRateDto>> GetMachineStateRateByMac(GetMachineStateRateInputDto input);

        Task<RealtimeMachineStateSummaryDto> GetRealtimeMachineStateSummary();

        Task<IEnumerable<GetMachineShiftSolutionsDto>> GetMachineShiftSolutions(dynamic input);

        Task<FileDto> Export(GetMachineStateRateInputDto input);
    }
}