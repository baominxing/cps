using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Carton.CartonTraceability.Dtos;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Carton.CartonTraceability
{
    public interface ICartonTraceabilityAppService : IApplicationService
    {
        Task<IEnumerable<CartonRecordDto>> ListPartsInCarton(EntityDto<int> input);

        Task<PagedResultDto<CartonTraceabilityDto>> ListTraceabilityRecords(CartonTraceabilityRequestDto input);

        Task<FileDto> ExportTraceabilityRecords(CartonTraceabilityRequestDto input);

        Task<IEnumerable<CartonRecordDto>> ListCartonRecords(CartonRecordRequestDto input);

        Task<bool> CheckUndoPermission(UndoPackingPermissionDto input);

        Task Delete(EntityDto<int> input);
    }
}
