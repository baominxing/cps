namespace Wimi.BtlCore.StaffPerformances
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Application.Services;

    using Wimi.BtlCore.StaffPerformances.Dto;

    public interface IStaffPerformanceAppService : IApplicationService
    {
        Task<IEnumerable<StaffPerformanceDto>> GetScrollTab(StaffPerformanceRequestDto input);

        Task<IEnumerable<StaffPerformanceDto>> GetStaffList();

        Task<IEnumerable<dynamic>> GetShiftSolutionList();

        Task<StaffPerformanceDto> GetStaffPerformance(StaffPerformanceRequestDto input);
    }
}