using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Maintain.Dto;

namespace Wimi.BtlCore.Maintain
{
    public interface IRepairRequestAppService : IApplicationService
    {
        /// <summary>
        /// 获取所有的用户
        /// </summary>
        /// <returns></returns>
        Task<IList<NameValueDto>> ListUser();
        /// <summary>
        /// 获取设备类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MaintianMachineTypeDto> GetMachineType(EntityDto input);

        Task CreateOrUpdate(MaintainRequestDto input);

        Task<PagedResultDto<MaintainRequestDto>> ListRequest(MainRequestFilterDto input);

        Task DeleteRequest(EntityDto input);

        MaintainRequestDto GetMaintainRequest(EntityDto input);

        MaintainRequestDto GetMaintainRequestOnLook(EntityDto input);

        MaintainRepairDto GetMaintainRepairList(EntityDto input);

        Task LookOrRepair(RepairInputDto input);
    }
}
