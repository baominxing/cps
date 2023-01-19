using System.Threading.Tasks;

using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Order.LoginReports.Dtos;

namespace Wimi.BtlCore.Order.LoginReports
{
    public interface ILoginReportAppService : IApplicationService
    {
        Task Close(EntityDto input);

        Task<MachineReportDto> GetDefectiveReasonsForMachineReport(EntityDto input);

        Task<WorkOrderReportDto> GetOutputQuantityForOrderReport(EntityDto input);

        Task<DatatablesPagedResultOutput<WorkOrderDto>> ListWorkOrders(WorkOrdersRequestDto input);

        Task<DatatablesPagedResultOutput<WorkOrderTaskDto>> ListWorkOrderTasks(WorkOrderTasksRequestDto input);

        Task Login(WorkOrderLoginDto input);

        Task MachineReport(MachineReportDto input);

        Task WorkOrderReport(WorkOrderReportDto input);
    }
}
