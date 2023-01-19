using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Tongs.Dto;

namespace Wimi.BtlCore.Tongs
{
    public interface ITongAppService : IApplicationService
    {
        Task<DatatablesPagedResultOutput<TongDto>> ListTong(TongInputDto input);

        Task CreateTong(TongInputDto input);

        Task UpdateTong(TongInputDto input);

        Task DeleteTong(EntityDto input);

        Task<TongDto> GetTongForEdit(TongInputDto input);

        Task<List<NameValueDto>> GetTongsForSelect();
    }
}
