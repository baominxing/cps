using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Wimi.BtlCore.ThirdpartyApis.Dto;

namespace Wimi.BtlCore.ThirdpartyApis.Interfaces
{
    public interface IStatisticalComponentManager : IDomainService
    {
        Task<ApiResponseObject> ListHourlyMachineYiled(string workShopCode);

        Task<ApiResponseObject> ListPerHourYields(string workShopCode);

        Task<ApiResponseObject> ListHourlyMachineYiledByShiftDay(string workShopCode);

        Task<ApiResponseObject> ListCurrentShiftCapcity(List<CurrentMachineShiftInfoDto> machineShiftDetails);

        Task<ApiResponseObject> ListMachineActivation(string workShopCode, List<int> currentMachineShiftDetailList);

        Task<ApiResponseObject> ListMachineActivationByDay(string workShopCode);
    }
}