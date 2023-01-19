using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Trace.Dto;
using Wimi.BtlCore.Trace.Repository.Dtos;
using Wimi.BtlCore.Traceability.Dto;

namespace Wimi.BtlCore.Traceability
{
    public interface ITraceAppService : IApplicationService
    {
        Task<DatatablesPagedResultOutput<TraceLineFlowSettingSummaryDto>> ListLineFlowSettings(QueryTraceLineFlowSettingDto input);

        Task<List<NameValueDto>> ListLineFlowSettingsByGroupId(EntityDto deviceGroup);

        List<NameValueDto> ListStationType();

        List<NameValueDto> ListFlowType();

        Task<FlowSettingsDetailDto> GetFlowSettingsDetail(EntityDto input);

        Task AddMachineIntoTraceFlowSetting(AddMachineIntoTraceFlowSettingDto input);

        Task RemoveMachineFromTraceFlowSetting(RemoveMachineFromTraceFlowSettingDto input);

        Task UpdateTraceFlowSetting(TraceFlowSettingDto traceFlowSettingDto);

        Task<FlowSettingsDetailDto> SaveTraceFlowSetting(TraceFlowSettingDto traceFlowSettingDto);

        Task DeleteTraceFlowSetting(EntityDto traceFlowSetting);

        Task<IEnumerable<NameValueDto>> ListMachines(EntityDto deviceGroup);

        Task<DatatablesPagedResultOutput<TraceCatalogDto>> ListTraceCatalog(QueryTraceCatalogsDto input);

        Task<TracePartDetailDto> ListTraceRecordByPartNo(QueryTraceCatalogsDto input);

        Task<ProcessParamterDto> ListProcessParamters(ProcessParamterRequestDto input);

        Task<string> GetTraceFlowExtensionData(EntityDto flow);

        Task<PagedResultDto<NgPartsResultDto>> ListNgPartsRecord(NgPartsRequestDto input);

        Task<PagedResultDto<PartDefectDetailInfoDto>> ListDefectiveInfos(NgPartsRequestDto input);

        Task SaveCollectionDefects(PartDefectsCreateDto input);

        IEnumerable<NameValueDto> ListDefectiveParts();

        IEnumerable<NameValueDto> ListDefectiveReasonsByPartId(EntityDto input);

        Task<IEnumerable<NameValueDto>> ListDeviceGroups();

        IEnumerable<NameValueDto> ListDeviceGroupMachines(EntityDto input);

        IEnumerable<NameValueDto> ListShift();

        Task<FileDto> Export(QueryTraceCatalogsDto input);

        Task<FileDto> ExportNgParts(NgPartsRequestDto input);
    }
}
