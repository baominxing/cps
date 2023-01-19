using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Wimi.BtlCore.Authorization.DmpMachineSetting.Dto;

namespace Wimi.BtlCore.Authorization.DmpMachineSetting
{
    public interface IDmpMachineAppService : IApplicationService
    {
        Task<ListResultDto<DmpDto>> GetDmps();

        Task<PagedResultDto<GetDmpMachinesOutputDto>> GetDmpMachines(GetDmpMachinesInputDto input);

        Task<string> AddMachineListToDmp(MachineListToDmpInputDto input);

        Task RemoveMachineFromDmp(RemoveMachineFromDmpInputDto input);

        Task BatchRemoveMachineFromDmp(BatchRemoveMachineFromDmpInputDto input);
    }
}
