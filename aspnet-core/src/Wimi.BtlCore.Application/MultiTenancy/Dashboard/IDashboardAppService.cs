using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Dashboard.Dtos;
using Wimi.BtlCore.MultiTenancy.Dashboard.Dto;
using Wimi.BtlCore.StatisticAnalysis.States.Dto;

namespace Wimi.BtlCore.MultiTenancy.Dashboard
{
    public interface IDashboardAppService : IApplicationService
    {
        Task<GetCurrentMachineShiftInfoDto> GetCurrentMachineShiftInfo();

        Task<StatesStatisticForChartDto> GetDashboardStatisticForChartInGivenDays(GetStatesStatisticInGivenDaysInputDto input);

        Task<StatesStatisticForChartDto> GetDashboardStatisticForChartInGivenDaysByGroupId(GetStatesStatisticInGivenDaysInputDto input);

        Task<IEnumerable<MachineStatisticDataDto>> GetDashboardStatisticInGivenDays(GetStatesStatisticInGivenDaysInputDto input);

        Task<IEnumerable<MachineStatisticDataDto>> GetDashboardStatisticInGivenDaysByGroupId(GetStatesStatisticInGivenDaysInputDto input);

        Task<GetMachineActivationDto> GetMachineActivationForDashboard();

        Task<MachineStatisticDataDto> GetMachineRealTimePadData(GetMachineRealTimePadDataDto input);

        Task<GetMachineUsedTimeRateDto> GetMachineUsedTimeRateForDashboard();

        GetMemberActivityOutputDto GetMemberActivity();
    }
}