using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.StatisticAnalysis.States.Dto;
using Wimi.BtlCore.StatisticAnalysis.Yield.Dto;

namespace Wimi.BtlCore.StatisticAnalysis.Yield
{
    public interface IYieldAppService : IApplicationService
    {
        /// <summary>
        ///     获取第一次查询时的参数，前台获取不到
        /// </summary>
        /// <returns></returns>
        Task<GetMachineYieldQueryParamDto> GetFirstQueryParam(QueryDateTimeDto input);

        /// <summary>
        ///     获取某台设备当天各程序的平均生产节拍和产量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<Yield4PerProgramOutputDto>> GetMachineAvgProgramDurationAndYield(MachineDetailYieldInfoInputDto input);

        Task<IEnumerable<GetMachineGanttChartOutputDto>> GetMachineDetailGanttCharat(MachineDetailYieldInfoInputDto input);

        /// <summary>
        ///     设备各状态比率
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<MachineStateRateOutputDto>> GetMachineStateRate(MachineDetailYieldInfoInputDto input);

        /// <summary>
        ///     获取设备甘特图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<GetMachineGanttChartOutputDto>> GetMachineStatesGanttChart(MachineDetailYieldInfoInputDto input);

        /// <summary>
        /// 获取设备利用率
        /// </summary>
        /// <param name="input">
        /// </param>
        /// <returns>
        /// </returns>
        Task<IEnumerable<UtilizationRateOutputDto>> GetMachineUtilizationRate(MachineDetailYieldInfoInputDto input);

        Task<IEnumerable<UtilizationRateOutputDto>> GetMachineUtilizationRate4Popover(MachineDetailYieldInfoInputDto input);

        /// <summary>
        ///     获取设备产量效率总数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<MachineYieldAnalysisOutputDto>> GetMachineYieldAnalysis(GetMachineStateRateInputDto input);

        Task<IEnumerable<MachineStateDto>> GetSummaryDate(GetMachineStateRateInputDto input);

        GetMachineStateRateInputDto GetStartTimeOfGanttChart(GetMachineStateRateInputDto input);

        /// <summary>
        /// 产量统计页面数据接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MachineYieldDto> GetMachineCapability(GetMachineYieldInputDto input);

        Task<FileDto> Export(GetMachineYieldInputDto input);
    }
}