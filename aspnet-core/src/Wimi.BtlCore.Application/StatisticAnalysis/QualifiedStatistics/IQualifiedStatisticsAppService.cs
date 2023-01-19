using System.Collections.Generic;
using Abp.Application.Services;
using System.Threading.Tasks;
using Wimi.BtlCore.StatisticAnalysis.QualifiedStatistics.Dto;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.BasicData.Shifts.Manager.Dto;

namespace Wimi.BtlCore.StatisticAnalysis.QualifiedStatistics
{
    public interface IQualifiedStatisticsAppService : IApplicationService
    {
        Task<ListQualificationInfoResultDto> ListQualificationInfo(GetDeviceGroupQualifiedRateRequestDto input);

        Task<FileDto> Export(GetDeviceGroupQualifiedRateRequestDto input);

        Task<IEnumerable<GetMachineShiftSolutionsDto>> ListDeviceGroupSolution(GetDeviceGroupQualifiedRateRequestDto input);

    }
}
