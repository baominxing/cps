using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Wimi.BtlCore.CraftMaintain.Dtos;

namespace Wimi.BtlCore.CraftMaintain
{
    public interface IFmsCutterSettingAppService : IApplicationService
    {
        Task<IEnumerable<FmsCutterSettingItemDto>> ListColumns();

        Task CreateOrUpdate(FmsCutterSettingItemDto input);

        Task<FmsCutterSettingDto> GetSettingDto();

        Task Update(IEnumerable<NameValueDto<int>> input);
    }
}