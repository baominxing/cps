using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.CraftMaintain.Dtos;

namespace Wimi.BtlCore.CraftMaintain
{
    public interface IFlexibleCraftMaintainAppService : IApplicationService
    {
        Task<List<GetCraftsDto>> GetCrafts(GetCraftsInput input);
        Task<GetCraftCuttersDto> GetCraftCutters(GetCraftCuttersInput input);
        Task<StepOneDto> StepOne(EntityDto input);
        Task<List<CraftPathCutterDto>> StepTwo(StepTwoInput input);
        Task<FlexibleCraftProcesseDto> CreateOrEditCraftProcesse(CreateCraftProcesseInput input);
        Task CreatOrEditCraft(CreatCraftInput input); 
        Task<PagedResultDto<FlexibleCraftProcesseDto>> GetCraftProcesses(GetCraftProcessesInput input); 
        Task DeleteCraft(EntityDto input); 
        Task<CraftPathMapData> GetCraftPathMapData(EntityDto input);
        Task<List<GetAllCraftsDto>> GetAllCrafts(GetCraftsInput input);
    }
}
