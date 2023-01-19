// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMachineGatherParamAppService.cs" company="WimiSoft">
//   WimiSoft Copyright 2017
// </copyright>
// <summary>
//   Defines the IMachineGatherParamAppService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Wimi.BtlCore.BasicData
{
    using Abp.Application.Services;
    using Abp.Application.Services.Dto;
    using System.Threading.Tasks;
    using Wimi.BtlCore.BasicData.Dto;

    public interface IMachineGatherParamAppService : IApplicationService
    {
        Task<PagedResultDto<MachineGatherParamsOutputDto>> GetMachineGatherParams(GetGatherParamsInputDto input);

        Task BatchSwitchs(BatchSwitchDto input);
    }
}