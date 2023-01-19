using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.BasicData.Shifts.Manager.Dto;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.StatisticAnalysis.Alarms.Dto;
 
namespace Wimi.BtlCore.StatisticAnalysis.Alarms
{
    public interface IAlarmsAppService : IApplicationService
    {
        Task DeleteAlarmInfo(NullableIdDto input);

        Task<IEnumerable<MachineAlarmStatisticesDto>> GetAlarmChartData(GetMachineAlarmsInputDto input);

        Task<int> GetAlarmChartDataCount(GetMachineAlarmsInputDto input);

        Task<IEnumerable<MachineAlarmStatisticesDto>> GetAlarmDetailData(GetMachineAlarmsInputDto input);

        Task<IEnumerable<MachineAlarmStatisticesDto>> GetAlarmDetailDataForModal(GetMachineAlarmsInputDto input);

        Task<PagedResultDto<CreateOrEditAlarmInfoDto>> GetAlarmInfoList(GetAlarmInfoListInputDto input);

        Task<IEnumerable<MachineAlarmStatisticesDto>> GetDefaultAlarmChartData(GetMachineAlarmsInputDto input);

        Task<IEnumerable<GetMachineShiftSolutionsDto>> GetMachineShiftSolutions(dynamic input);

        /// <summary>
        /// 完全覆盖
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task ImportDataByCover(ImportDataDto input);

        /// <summary>
        /// 增量新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task ImportDataByIncrement(ImportDataDto input);

        Task UpdateOrEditAlarmInfo(CreateOrEditAlarmInfoDto input);

        Task<FileDto> Export(GetMachineAlarmsInputDto input);
    }
}