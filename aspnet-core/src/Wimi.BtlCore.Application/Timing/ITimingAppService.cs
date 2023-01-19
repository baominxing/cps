using System.Collections.Generic;
using System.Threading.Tasks;

using Abp.Application.Services;
using Abp.Application.Services.Dto;

using Wimi.BtlCore.Timing.Dto;

namespace Wimi.BtlCore.Timing
{
    public interface ITimingAppService : IApplicationService
    {
        Task<List<ComboboxItemDto>> GetEditionComboboxItems(GetTimezoneComboboxItemsInputDto input);

        Task<ListResultDto<NameValueDto>> GetTimezones(GetTimezonesInputDto input);
    }
}