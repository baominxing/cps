namespace Wimi.BtlCore.StaffPerformances
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Application.Services;
    using Abp.Application.Services.Dto;
    using Wimi.BtlCore.StaffPerformances.Dto;

    public interface IStaffPerformanceYieldAppService : IApplicationService
    {
        Task<IEnumerable<NameValueDto<int>>> AllMachines(AllMachinesRquestDto input);

        Task<IEnumerable<NameValueDto<long>>> AllUsers();

        Task<ProductionChartResultDto> ProductionChart(ProductionChartRequestDto input);
    }
}