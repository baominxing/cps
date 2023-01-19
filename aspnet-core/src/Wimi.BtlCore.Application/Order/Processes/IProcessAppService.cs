using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using Wimi.BtlCore.Order.Processes.Dtos;

namespace Wimi.BtlCore.Order.Processes
{
    public interface IProcessAppService : IApplicationService
    {
        Task CreateOrUpdateProcess(ProcessDto input);

        Task DeleteProcess(EntityDto input);

        Task<PagedResultDto<ProcessDto>> GetProcess(ProcessFilterDto input);
    }
}
