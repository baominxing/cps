namespace Wimi.BtlCore.StaffPerformances
{
    using System.Threading.Tasks;

    using Abp.Application.Services;
    using Abp.Application.Services.Dto;

    using Wimi.BtlCore.StaffPerformances.Dto;

    public interface IOnlineOrOfflineRecordAppService : IApplicationService
    {
        Task<PagedResultDto<OnlineAndOfflineRecordDto>> QueryRecords(QueryRecordsDto input);

        Task<ListResultDto<NameValueDto<long>>> ListUsers(EntityDto input);

        Task<ListResultDto<NameValueDto<int>>> ListDeviceGroups();

    }
}