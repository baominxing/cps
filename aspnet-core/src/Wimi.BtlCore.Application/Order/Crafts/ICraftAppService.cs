using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wimi.BtlCore.Order.Crafts.Dtos;

namespace Wimi.BtlCore.Order.Crafts
{
    public interface ICraftAppService : IApplicationService
    {
        Task<bool> CraftIsInProcess(EntityDto input);

        Task CreateCraft(CraftRequestDto input);

        Task CreateCraftProcess(CraftRequestDto input);

        Task DeleteCraft(EntityDto input);

        Task DeleteCraftProcess(CraftProcessRequestDto input);

        Task<CraftDto> GetCraftForEdit(CraftRequestDto input);

        Task<PagedResultDto<CraftProcessDto>> GetCraftProcesses(CraftRequestDto input);

        Task<IEnumerable<CraftDto>> GetCrafts();

        Task UpdateCraft(CraftRequestDto input);

        Task UpdateCraftProcess(List<CraftProcessRequestDto> input);
    }
}
