using System.Threading.Tasks;

using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Order.ProductionPlans.Dtos;

namespace Wimi.BtlCore.Order.ProductionPlans
{
    public interface IProductionPlanAppService : IApplicationService
    {
        Task<bool> CanCreate();

        Task<ChangeWorkOrderVolumeDto> ChangeWorkOrderVolume(ChangeWorkOrderVolumeDto input);

        Task CreateOrUpdate(ProductionPlanDto input);

        Task Delete(EntityDto input);

        Task<GetProductionPlanForEditDto> GetProductionPlanForEdit(NullableIdDto input);

        Task<DatatablesPagedResultOutput<ProductionPlanListDto>> ListProductionPlan(ListProductionPlanDto listProductionPlanDto);
    }
}
