using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Shifts.Manager.Dto;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.EfficiencyTrendas.Dtos;
using Wimi.BtlCore.StatisticAnalysis.EfficiencyTrends.Dto;

namespace Wimi.BtlCore.StatisticAnalysis.EfficiencyTrends
{
    public interface IEfficiencyTrendsAppService : IApplicationService
    {
        List<EfficiencyTrendasDataTablesDto> GetEfficiencyTrendasDataTablesColumns(EfficiencyTrendsInputDto input);

        /// <summary>
        ///     DataTables表格数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ListResultDto<EfficiencyTrendasDataTablesDataDto>> GetEfficiencyTrendasList(EfficiencyTrendsInputDto input);

        /// <summary>
        /// 获取设备稼动率
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ListResultDto<EfficiencyTrendasDataTablesDataDto>> GetMachineActivation(EfficiencyTrendsInputDto input);

        Task<IEnumerable<GetMachineShiftSolutionsDto>> GetMachineShiftSolutions(dynamic input);

        Task<IEnumerable<NameValueDto<int>>> GetDefaultMachines();

        Task<FileDto> Export(EfficiencyTrendsInputDto input);

        /// <summary>
        /// 获取设备稼动率原始数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //Task<List<ExpandoObject>> GetMachineActivationOriginalData(EfficiencyTrendsInputDto input);
    }
}