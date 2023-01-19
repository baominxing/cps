namespace Wimi.BtlCore.Cutter
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Application.Services;
    using Abp.Application.Services.Dto;

    using Wimi.BtlCore.Cutter.Dto;
    using Wimi.BtlCore.Dto;

    public interface ICutterAppService : IApplicationService
    {
        Task BulkUnLoadCutters(LoadOrUnLoadCuttersDto input);

        Task CreateOrUpdateCutterModel(CutterModelDto input);

        Task CreateOrUpdateCutterParameter(CutterParameterDto input);

        Task CreateOrUpdateCutterStates(CreateUpdateCutterStatesDto input);

        Task<CutterTypeDto> CreateOrUpdateCutterType(CutterTypeDto input);

        Task CutterLoadOrUnLoad(LoadOrUnLoadCuttersDto input);

        Task DeleteCutterModel(CutterModelDto input);

        Task DeleteCutterParameter(CutterParameterDto input);

        Task DeleteCutterStates(EntityDto input);

        Task DeleteCutterType(CutterTypeDto input);

        /// <summary>
        /// 获取刀具型号键值对
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// </returns>
        Task<IEnumerable<NameValueDto<int>>> FindCutterModal(QueryCutterStateDto input);

        /// <summary>
        /// 获取刀具类型树结构
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CutterTypeDto>> FindCutterType();

        Task<CutterTypeDto> FindCutterTypeById(EntityDto input);

        Task<GetCutterModelDefaultDto> GetCutterModelDefaultValue(EntityDto input);

        Task<CutterModelDto> GetCutterModelForEdit(CutterModelDto input);

        Task<IEnumerable<CutterModelDto>> GetCutterModelList(CutterModelDto input);

        Task<ListResultDto<CutterModelDto>> GetCutterModels();

        Task<CutterParameterDto> GetCutterParameterForEdit(CutterParameterDto input);
        
        Task<IEnumerable<CutterParameterDto>> GetCutterParameterList();

        /// <summary>
        /// 获取刀具状态列名
        /// </summary>
        /// <returns></returns>
        Task<GetCutterStatesColumnsDto> GetCutterStatesColumns();

        /// <summary>
        /// 获取刀具状态表格查询
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// </returns>
        Task<DatatablesPagedResultOutput<CutterStatesDto>> GetCutterStatesList(QueryCutterStateDto input);

        Task<CutterTypeDto> GetCutterTypeForEdit(CutterTypeDto input);

        Task<IEnumerable<CutterTypeDto>> GetCutterTypeList();

        Task<ListResultDto<CutterParameterDto>> GetDynamicColumns();

        Task<PagedResultDto<CutterLoadAndUnloadRecordDto>> QueryCutterLoadAndUnloadRecords(QueryCutterRecordDto input);

        Task<IEnumerable<ListLoadingMachineCuttersDto>> ListLoadingMachineCutters();

        Task<IEnumerable<ListLoadingMachineCuttersDto>> ListLoadingMachines();

        Task<IEnumerable<LoadingMachineCutterDetails>> ListMachineCutterDetails(EntityDto<string> input);

        Task SaveMachineCutterRates(SaveMachineCutterRatesInputDto input);

        Task ResetCutterLife(EntityDto input);

        Task<FileDto> ExportCutterLoadAndUnloadRecords(QueryCutterRecordDto input);

        Task<FileDto> ExportCutterStatesList(QueryCutterStateDto input);
    }
}