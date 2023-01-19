namespace Wimi.BtlCore.ThirdpartyApis
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Application.Services;
    using Wimi.BtlCore.DeviceGroups.Dto;
    using Wimi.BtlCore.ThirdpartyApis.Dto;

    public interface IVisualSettingAppService : IApplicationService
    {
        Task<IEnumerable<WorkShopDto>> ListWorkShops();

        Task<IEnumerable<DeviceGroupDto>> ListDeviceGroups(RequestDto input);

        Task<IEnumerable<ThirdpartyApiDto>> ListThirdpartyApis();

        string ReadConfig(RequestDto input);

        bool SaveConfig(VisionSettingFileDto input);

        void DeletePicture(VisionSettingFileDto input);

        IEnumerable<VisionSettingFileDto> ListPictures();

        string ReadComponentConfig();

        bool SaveComponentConfig(VisionSettingFileDto input);
    }
}