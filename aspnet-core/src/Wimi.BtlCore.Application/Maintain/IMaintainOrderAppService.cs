using Abp.Application.Services;
using System.Threading.Tasks;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Maintain.Dto;

namespace Wimi.BtlCore.Maintain
{
    public interface IMaintainOrderAppService : IApplicationService
    {
        Task<DatatablesPagedResultOutput<MaintainOrderDto>> ListMaintainOrder(MaintainOrderInputDto input);

        MaintainOrderDto GetMaintainOrder(MaintainOrderInputDto input);

        Task Update(MaintainOrderInputDto input);
    }
}
