using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Wimi.BtlCore.CustomFields.Dto;

namespace Wimi.BtlCore.CustomFields
{
    public interface ICustomFieldAppService : IApplicationService
    {
        Task Delete(EntityDto<List<int>> input);

        Task CreateOrUpdate(CustomFieldDto input);
    }
}