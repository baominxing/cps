namespace Wimi.BtlCore.BackgroundJobs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Application.Services;
    using Microsoft.AspNetCore.Mvc;
    using Wimi.BtlCore.BackgroundJobs.Dto;

    public interface IBackgroundJobAppService : IApplicationService
    {

        Task RuningStateSplitByShift(IEnumerable<MachineShiftDetailDto> input);

        Task<bool> RuningStateSplitByHourNaturalDay(IEnumerable<MachineShiftDetailDto> input);

        [HttpGet]
        void RunningDmpJobAboutShift();
    }


}