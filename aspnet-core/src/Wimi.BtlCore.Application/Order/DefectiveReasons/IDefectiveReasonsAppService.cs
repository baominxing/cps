using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Order.DefectiveReasons.Dtos;

namespace Wimi.BtlCore.Order.DefectiveReasons
{
    public interface IDefectiveReasonsAppService : IApplicationService
    {
        Task CreateOrUpdateDefectiveReason(DefectiveReasonInputDto input);

        Task DeleteDefectiveReason(EntityDto input);

        Task<PagedResultDto<DefectiveReasonDto>> ListDefectiveReasons(DefectiveReasonFilterDto input);

        Task<ListResultDto<DefectivePartDto>> ListDefectivePart();


        Task DeleteDefectivePart(EntityDto input);

        Task<DefectivePartDto> CreateDefectivePart(CreateDefectivePartDto input);

        Task<DefectivePartDto> UpdateDefectivePart(UpdateDefectivePartDto input);

        Task<DefectivePartDto> MoveDefectivePart(MoveDefectivePartDto input);

        Task<IEnumerable<DefectiveReasonDto>> ListDefectiveReasonsByPartId(EntityDto input);


    }
}
