using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.AppSystem.IO;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.DeviceGroups.Dto;
using Wimi.BtlCore.ThirdpartyApis.Dto;

namespace Wimi.BtlCore.ThirdpartyApis
{
    [AbpAllowAnonymous]
    public class VisualSettingAppService : BtlCoreAppServiceBase, IVisualSettingAppService
    {
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<DeviceGroup> deviceGroupRepository;
        private readonly IRepository<ThirdpartyApi, Guid> thirdpartyApiRepository;
        private readonly ISettingManager settingManager;
        private readonly IBtlFolders appFolders;
        private readonly IMachineManager machineManager;
        private readonly DeviceGroupManager deviceGroupManager;


        public VisualSettingAppService(DeviceGroupManager deviceGroupManager, IMachineManager machineManager, IRepository<Machine> machineRepository, IRepository<MachineDeviceGroup> machineDeviceGroupRepository, IRepository<DeviceGroup> deviceGroupRepository, IBtlFolders appFolders, IRepository<ThirdpartyApi, Guid> thirdpartyApiRepository, ISettingManager settingManager)
        {
            this.machineManager = machineManager;
            this.machineRepository = machineRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.deviceGroupRepository = deviceGroupRepository;
            this.appFolders = appFolders;
            this.thirdpartyApiRepository = thirdpartyApiRepository;
            this.settingManager = settingManager;
            this.deviceGroupManager = deviceGroupManager;
        }

        [HttpPost]
        public async Task<IEnumerable<WorkShopDto>> ListWorkShops()
        {
            // 根级组作为车间组，在看板上分类显示
            var rootDeviceGroups = (await deviceGroupManager.FindChildrenAsync(null, false)).OrderBy(s => s.Seq);

            return rootDeviceGroups.Select(rootDeviceGroup => new WorkShopDto
            {
                Id = rootDeviceGroup.Id,
                Code = rootDeviceGroup.Code,
                Name = rootDeviceGroup.DisplayName
            })
                .ToList();
        }

        [HttpPost]
        public async Task<IEnumerable<DeviceGroupDto>> ListDeviceGroups([FromHeader]RequestDto input)
        {
            var targetGroup = deviceGroupRepository.FirstOrDefault(q => q.Code == input.WorkShopCode);
            if (targetGroup == null)
            {
                throw new UserFriendlyException(this.L("WorkshopDoesNotExist"));
            }

            var deviceGroups = (await deviceGroupManager.FindChildrenAsync(targetGroup.Id, true)).OrderBy(t => t.Seq);

            return ObjectMapper.Map<IEnumerable<DeviceGroupDto>>(deviceGroups);
        }

        [HttpPost]
        public async Task<IEnumerable<ThirdpartyApiDto>> ListThirdpartyApis()
        {
            var apis = await this.thirdpartyApiRepository.GetAll()
                           .Select(
                               n => new ThirdpartyApiDto()
                               {
                                   Code = n.Code,
                                   Name = n.Name,
                                   Url = n.Url
                               }).ToListAsync();

            return apis;
        }

        [HttpPost]
        public string ReadConfig([FromHeader]RequestDto input)
        {
            var sourcestring = string.Empty;
            if (input.WorkShopCode.IsNullOrEmpty())
            {
                var defaultWorkShop = this.deviceGroupRepository.GetAll().Where(q=>q.ParentId == null).OrderBy(w => w.Id).FirstOrDefault();
                if (defaultWorkShop == null) return sourcestring;
                input.WorkShopCode = defaultWorkShop.Code;
            }

            var filename = $"{this.appFolders.ConfigurationsFloder}/{input.WorkShopCode}.json";
            if (File.Exists(filename))
            {
                sourcestring = File.ReadAllText(filename, Encoding.UTF8);
            }

            return sourcestring;
        }

        [HttpPost]
        public bool SaveConfig([FromHeader]VisionSettingFileDto input)
        {
            try
            {
                var filename = $"{this.appFolders.ConfigurationsFloder}/{input.FileName}.json";
                AppFileHelper.DeleteFilesInFolderIfExists(this.appFolders.ConfigurationsFloder, input.FileName);
                File.WriteAllText(filename, input.Content, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(this.L("WriteDataFailed{0}", ex.Message));
            }

            return true;
        }

        [HttpPost]
        public void DeletePicture([FromHeader]VisionSettingFileDto input)
        {
            AppFileHelper.DeleteFilesInFolderIfExists(this.appFolders.VisualImgFloder, input.Name);
        }

        [HttpPost]
        public IEnumerable<VisionSettingFileDto> ListPictures()
        {
            var directory = new DirectoryInfo(this.appFolders.VisualImgFloder);
            var files = directory.GetFiles("*", SearchOption.AllDirectories);
            return files.Select(
                f => new VisionSettingFileDto()
                {
                    FullPath = $"/{AppPath.VisualImage}/{f.Name}",
                    WorkShopCode = f.Name
                });
        }

        [HttpPost]
        public string ReadComponentConfig()
        {
            var sourcestring = string.Empty;
            var filename = $"{this.appFolders.ComponentConfigFolder}/Setting.json";
            if (File.Exists(filename))
            {
                sourcestring = File.ReadAllText(filename, Encoding.UTF8);
            }

            return sourcestring;
        }

        [HttpPost]
        public bool SaveComponentConfig([FromHeader]VisionSettingFileDto input)
        {
            try
            {
                var filename = $"{this.appFolders.ComponentConfigFolder}/Setting.json";
                AppFileHelper.DeleteFilesInFolderIfExists(this.appFolders.ComponentConfigFolder, input.FileName);
                File.WriteAllText(filename, input.Content, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(this.L("WriteDataFailed{0}", ex.Message));
            }

            return true;
        }
    }
}