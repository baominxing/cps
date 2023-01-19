using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Wimi.BtlCore.DeviceGroups.Dto;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.StatisticAnalysis.StatusDistributionMap.Dto;

namespace Wimi.BtlCore.StatisticAnalysis.StatusDistributionMap
{
    public interface IStatusDistributionMapAppService : IApplicationService
    {
        Task<IEnumerable<NameValueDto<int>>> ListMachines(DeviceGroupDto input);

        Task<IEnumerable<NameValueDto<int>>> ListDeviceGroups();

        Task UpdateMemo(StatusDistributionItemDto input);    
            
        Task<DatatablesPagedResultOutput<StatusDistributionItemDto>> ListStatusByMachine(StatusDistributionDetailRequireDto input);

        Task<DatatablesPagedResultOutput<StatusDistributionItemDto>> ListStatusByShiftDetail(StatusDistributionDetailRequireDto input);

        Task<IEnumerable<DailyStatusSummaryDto>> ListStatusSummary(DailyStatusSummaryRequireDto input);

        Task<IEnumerable<StatusDistributionDto>> ListStatusDistribution(StatusDistributionRequireDto input);

        Task<IEnumerable<ShiftStatusDistributionDto>> ListShiftStatusDistribution(DailyStatusSummaryRequireDto input);

        Task<IEnumerable<NameValueDto<int>>> ListSummaryConditions(StatusDistributionRequireDto input);

        Task<StatusDistributionItemDto> GetStateInfo(EntityDto input);
    }
}