namespace Wimi.BtlCore.StaffPerformances
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Application.Services;
    using Abp.Application.Services.Dto;

    using Wimi.BtlCore.StaffPerformances.Dto;

    public interface IPerformanceDeviceAppService : IApplicationService
    {
        Task<ListResultDto<GetDevicesRequestDto>> GetDevices(GetDevicesDto input);

        Task<ListResultDto<NameValueDto<int>>> GetMachineShiftDetail(EntityDto input);

        Task Offline(PersonnelOnDeviceDto input);

        Task Online(PersonnelOnDeviceDto input);

        Task<string> OnlineAll(PersonnelOnDeviceDto input);

        Task<string> OfflineBatch(PersonnelOnDeviceDto input);

        Task<IEnumerable<NameValueDto<int>>> ListShiftDetailByDeviceGroupId(EntityDto input);
    }
}