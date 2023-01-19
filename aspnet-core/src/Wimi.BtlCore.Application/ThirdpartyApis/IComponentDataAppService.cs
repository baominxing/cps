using Abp.Application.Services;
using System.Threading.Tasks;
using Wimi.BtlCore.ThirdpartyApis.Dto;

namespace Wimi.BtlCore.ThirdpartyApis
{
    public interface IComponentDataAppService : IApplicationService
    {
        Task<ApiResponseObject> ListRealtimeMachineInfos(RequestDto input);

        ApiResponseObject ListMachineAlarming(RequestDto input);

        Task<ApiResponseObject> ListPerHourYields(RequestDto input);

        Task<ApiResponseObject> ListHourlyMachineYiled(RequestDto input);

        Task<ApiResponseObject> ListHourlyMachineYiledByShiftDay(RequestDto input);

        Task<ApiResponseObject> ListNotices(RequestDto input);

        Task<ApiResponseObject> ListToolWarnings(RequestDto input);

        Task<ApiResponseObject> ListMachineStateDistribution(RequestDto input);

        Task<ApiResponseObject> ListMachineActivation(RequestDto input);

        Task<ApiResponseObject> ListMachineActivationByDay(RequestDto input);

        Task<ApiResponseObject> ListCurrentShiftCapcity(RequestDto input);

        Task<ApiResponseObject> ListGanttChart(RequestDto input);

        /// <summary>
        /// 产品生产计划达成率接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ApiResponseObject> ListPlanRate(RequestDto input);


        /// <summary>
        /// 设备实时状态,可配置参数项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ApiResponseObject> ListConfigRealtimeMachineInfos(RequestDto input);
    }
}
