using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Plan.ProcessPlans.Dtos;

namespace Wimi.BtlCore.Plan.ProcessPlans
{
    public interface IProcessPlanAppService : IApplicationService
    {
        Task CreateProcessPlan(CreatePlanDto processPlan);
        Task UpdateProcessPlan(CreatePlanDto processPlan);

        Task UpdateProcessPlanState(EditPlanDto processPlan);
        Task DeleteProcessPlan(int id);
        Task<DatatablesPagedResultOutput<PlanOutputDto>> ListPlan(PlanInputDto input);

        Task<List<ShiftItemDto>> GetCurrentShiftInfo(int deviceGroupId);

        Task<EditPlanDto> GetPlan(NullableIdDto input);

        IEnumerable<EditPlanDto> GetPlanParameterByMachineId(int planId);
        string IsInProcessing(int id);

        DatatablesPagedResultOutput<PlanOutputDto> GetPlanShiftItem(PlanInputDto input);

        Task<List<NameValueDto>> GetShiftSolutionName(ShiftSolutionNameInputDto input);


        Task<List<ShiftItemDto>> GetShiftInfo(GetShiftInfoInputDto input);
    }
}
