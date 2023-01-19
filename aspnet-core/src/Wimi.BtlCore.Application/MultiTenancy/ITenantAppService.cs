using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Wimi.BtlCore.MultiTenancy.Dto;

namespace Wimi.BtlCore.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

