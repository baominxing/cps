using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using Wimi.BtlCore.Order.StandardTimes.Dtos;

namespace Wimi.BtlCore.Order.StandardTimes
{
    public interface IStandardTimeAppService : IApplicationService
    {
        Task CreateOrUpdateStandardTime(StandardTimeDto input);

        Task DeleteStandardTime(EntityDto input);

        Task<PagedResultDto<StandardTimeDto>> GetStandardTime(StandardTimeFilterDto input);
    }
}
