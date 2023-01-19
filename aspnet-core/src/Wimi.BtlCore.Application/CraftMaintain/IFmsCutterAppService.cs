using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Wimi.BtlCore.CraftMaintain.Dtos;
using Wimi.BtlCore.CustomFields.Dto;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.FmsCutter.Dto;

namespace Wimi.BtlCore.CraftMaintain
{
    public interface IFmsCutterAppService : IApplicationService
    {
        Task<DatatablesPagedResultOutput<FmsCutterDto>> ListFmsCutter(FmsCutterInputDto input);

        Task CreateFmsCutter(FmsCutterDto input);

        Task UpdateFmsCutter(FmsCutterDto input);

        Task DeleteFmsCutter(FmsCutterDto input);

        Task<FmsCutterDto> GetFmsCutterForEdit(FmsCutterInputDto input);

        Task<IEnumerable<CustomFieldDto>> ListCutomFields();
    }
}
