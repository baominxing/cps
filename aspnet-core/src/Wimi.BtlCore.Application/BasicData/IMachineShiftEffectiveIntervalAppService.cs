using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Dto;

namespace Wimi.BtlCore.BasicData
{
    public interface IMachineShiftEffectiveIntervalAppService : IApplicationService
    {
        Task<IEnumerable<MachineShiftEffectiveIntervalDto>> ListShiftEffectiveIntervals();
    }
}