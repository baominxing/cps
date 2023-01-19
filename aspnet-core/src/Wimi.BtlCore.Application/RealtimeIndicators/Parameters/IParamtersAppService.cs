namespace Wimi.BtlCore.RealtimeIndicators.Parameters
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Application.Services;
    using Abp.Application.Services.Dto;
    using Wimi.BtlCore.BasicData.Dto;
    using Wimi.BtlCore.Common.Dto;
    using Wimi.BtlCore.Dto;
    using Wimi.BtlCore.Parameters.Dto;
    using Wimi.BtlCore.RealtimeIndicators.Parameters.Dto;

    public interface IParamtersAppService : IApplicationService
    {
        Task<MachineDto> GetDefaultMachineByTenantId(NullableIdDto input);

        Task<GetHistoryParamtersListDto> GetHistoryParamtersColumns(HistoryParamtersInputDto input);

        Task<PagedResultDto<GetHistoryParamtersDataTableDto>> GetHistoryParamtersList(HistoryParamtersInputDto input);

        Task<List<GetParamtersListDto>> GetLastNRecords(FindMachineInfoFromMongoInputDto input);

        IEnumerable<MachineAlarmDto> GetLodingMoreAlarm(AlarmPagesInputDto input);

        Task<MachineStatusListDto> GetMachineStatusDetail(FindMachineInfoFromMongoInputDto input);

        Task<MachineStateOutputDto> GetMachineStatusDetailList(MachineStateInputDto input);

        GetParamtersListDto GetParamtersList(ParamtersInputDto input);

        /// <summary>
        ///     获取当前实时报警信息
        /// </summary>
        /// <param name="input">输入Dto</param>
        /// <returns></returns>
        Task<MachineRealtimeAlarmOutDto> GetRealTimeAlarmList(AlarmPagesInputDto input);

        Task<IEnumerable<NameValueDto>> ListStates();

        Task<ParamComparisonOutput> ListParamComparisonChart(ParamComparisonInputDto input);

        Task<IEnumerable<NameValueDto>> ListNumberParamters(EntityDto input);

        Task<IEnumerable<ListMachineStatesDto>> ListMachineStates();

        Task<FileDto> Export(HistoryParamtersInputDto input);
    }
}