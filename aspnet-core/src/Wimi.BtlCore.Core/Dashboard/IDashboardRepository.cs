using Abp.Dependency;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Dashboard.Dtos;

namespace Wimi.BtlCore.Dashboard
{
    public interface IDashboardRepository: ITransientDependency
    {
        Task<IEnumerable<MachineStatisticDataDto>> GetDashboardStatisticInGivenDays(GetStatesStatisticInGivenDaysInputDto input);

        Task<IEnumerable<MachineStatisticDataDto>> GetDashboardStatisticInGivenDaysByGroupId(GetStatesStatisticInGivenDaysInputDto input);

        Task<MachineStatisticDataDto> GetMachineStatisticData(int machineIdList);

        Task<List<MachineUsedTimeRateDto>> QueryMachineUsedTimeRate(IEnumerable<int> currentMachineShiftId);

        Task<IEnumerable<int>> GetPreviousMachineShiftDetailList(List<int> currentMachineShiftDetailList);
    }
}
