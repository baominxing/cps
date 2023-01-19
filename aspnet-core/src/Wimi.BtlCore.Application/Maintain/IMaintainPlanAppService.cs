using Abp.Application.Services;
using System.Threading.Tasks;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Maintain.Dto;

namespace Wimi.BtlCore.Maintain
{
    public interface IMaintainPlanAppService : IApplicationService
    {
        Task<DatatablesPagedResultOutput<MaintainPlanDto>> ListMaintainPlan(MaintainPlanInputDto input);

        MaintainPlanDto GetMaintainPlan(MaintainPlanInputDto input);

        Task Delete(MaintainPlanInputDto input);

        Task Create(MaintainPlanInputDto input);

        Task Update(MaintainPlanInputDto input);

        Task StopMaintainPlan(MaintainPlanInputDto input);
    }
}
