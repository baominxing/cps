namespace Wimi.BtlCore.Web.Controllers.BasicData
{
    using Abp.AspNetCore.Mvc.Authorization;
    using Abp.Domain.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.Controllers;
    using Wimi.BtlCore.Web.Models.BasicData.DeviceGroups;

    public class DeviceGroupsController : BtlCoreControllerBase
    {
       
        private readonly IRepository<DeviceGroup> deviceGroupRepository;
        public DeviceGroupsController(IRepository<DeviceGroup> deviceGroupRepository)
        { 
            this.deviceGroupRepository = deviceGroupRepository;
        }

        [AbpMvcAuthorize(PermissionNames.Pages_BasicData_DeviceGroups_ManageDeviceTree)]
        public PartialViewResult CreateModal(int? parentId)
        {
            return this.PartialView("~/Views/BasicData/DeviceGroups/_CreateModal.cshtml", new CreateDeviceGroupModalViewModel(parentId));
        }

        [AbpMvcAuthorize(PermissionNames.Pages_BasicData_DeviceGroups_ManageDeviceTree)]
        public async Task<PartialViewResult> EditModal(int id)
        {
            var deviceGroup = await this.deviceGroupRepository.GetAsync(id);
            var model = ObjectMapper.Map<EditDeviceGroupModalViewModel>(deviceGroup);

            return this.PartialView("~/Views/BasicData/DeviceGroups/_EditModal.cshtml", model);
        }

        // GET: DeviceGroup
        public ActionResult Index()
        {
            return this.View("~/Views/BasicData/DeviceGroups/Index.cshtml");
        }
    }
}