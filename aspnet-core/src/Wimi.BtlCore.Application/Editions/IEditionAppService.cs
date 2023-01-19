using System.Collections.Generic;
using System.Threading.Tasks;

using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Editions.Dto;

namespace Wimi.BtlCore.Editions
{
    public interface IEditionAppService : IApplicationService
    {
        Task CreateOrUpdateEdition(CreateOrUpdateEditionDto input);

        Task DeleteEdition(EntityDto input);

        Task<List<ComboboxItemDto>> GetEditionComboboxItems(int? selectedEditionId = null);

        Task<GetEditionForEditOutputDto> GetEditionForEdit(NullableIdDto input);

        Task<PagedResultDto<EditionListDto>> GetEditions(GetEditionsInputDto input);

        //Task<ListResultDto<EditionListDto>> GetEditions();

        Task<FileDto> GetEditionToExcel();
    }
}