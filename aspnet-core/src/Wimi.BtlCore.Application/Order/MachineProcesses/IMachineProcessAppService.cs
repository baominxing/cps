using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wimi.BtlCore.Order.MachineProcesses.Dtos;

namespace Wimi.BtlCore.Order.MachineProcesses
{
    public interface IMachineProcessAppService : IApplicationService
    {
        Task<PagedResultDto<MachineProcessDto>> ListMachineProcess(MachineProcessPageDto filter);

        Task<IEnumerable<NameValueDto>> ListProductType();

        Task DeleteMachineProcess(EntityDto input);

        Task<IEnumerable<NameValueDto>> ListProcessType(EntityDto input);

        Task<string> CheckMachineRecord(MachineProcessDto input);

        Task ChangeMachineProduct(MachineProcessDto input);

    }
}
