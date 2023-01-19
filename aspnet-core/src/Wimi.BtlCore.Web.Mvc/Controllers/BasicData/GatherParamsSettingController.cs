namespace Wimi.BtlCore.Web.Controllers.BasicData
{
    using Abp.Application.Services.Dto;
    using Abp.AspNetCore.Mvc.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.BasicData;
    using Wimi.BtlCore.BasicData.Dto;
    using Wimi.BtlCore.Common;
    using Wimi.BtlCore.Configuration;
    using Wimi.BtlCore.Controllers;
    using Wimi.BtlCore.Web.Models.BasicData.GatherParamsSetting;

    [AbpMvcAuthorize(PermissionNames.Pages_BasicData_GatherParamsSetting)]
    public class GatherParamsSettingController : BtlCoreControllerBase
    {
      
        private readonly IBasicDataAppService basicdataAppService;

        private readonly ICommonLookupAppService commonLookupAppService;

        private readonly IMachineGatherParamAppService machineGatherParamAppService;

        public GatherParamsSettingController(
            IBasicDataAppService basicdataAppService, 
            ICommonLookupAppService commonLookupAppService, 
            IMachineGatherParamAppService machineGatherParamAppService
          )
        {
            this.commonLookupAppService = commonLookupAppService;
            this.machineGatherParamAppService = machineGatherParamAppService;
            this.basicdataAppService = basicdataAppService;
        }

        [AbpMvcAuthorize(PermissionNames.Pages_BasicData_GatherParamsSetting_Manage)]
        public async Task<PartialViewResult> EditModal(int? id)
        {
            var viewModel = await this.basicdataAppService.GetMachineGatherParamForEdit(new NullableIdDto { Id = id });

            var model = new GatherParamsSettingEditViewModel(viewModel)
            {
                YesNoModel = this.GetYesNoSelectListItems(),
                FixedDataValue = SettingManager.GetSettingValue(AppSettings.MachineParameter.FixedDataItems).Split(',')
            };

            return this.PartialView("~/Views/BasicData/GatherParamsSetting/_EditModal.cshtml", model);
        }


        public PartialViewResult BatchSwitchModal()
        {
            return this.PartialView("~/Views/BasicData/GatherParamsSetting/_BatchSwitchModal.cshtml");
        }

        public async Task<JsonResult> GetMachineGatherParams(GetGatherParamsInputDto input)
        {
            var pagedOutput = await this.machineGatherParamAppService.GetMachineGatherParams(input);
            return this.Json(pagedOutput);
        }

        public async Task<ActionResult> Index(int? tenantId)
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var modal = new GatherParamsSettingViewModel
                            {
                                Machines = deviceGroupAndMachineWithPermissions.Machines, 
                                DeviceGroups =
                                    deviceGroupAndMachineWithPermissions.DeviceGroups, 
                                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds
                            };
            return this.View("~/Views/BasicData/GatherParamsSetting/Index.cshtml", modal);
 
        }

        [AbpMvcAuthorize(PermissionNames.Pages_BasicData_GatherParamsSetting_Manage)]
        public async Task<PartialViewResult> SelectGroupModal(int id)
        {
            var deviceGroupAndMachineWithPermissions =
                await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();
            ViewBag.MachineId = id;
            var model = new GatherParamsSettingViewModel
            {
                Machines = deviceGroupAndMachineWithPermissions.Machines,
                DeviceGroups =
                                    deviceGroupAndMachineWithPermissions.DeviceGroups,
                GrantedGroupIds =
                                    deviceGroupAndMachineWithPermissions.GrantedGroupIds
            };
            return this.PartialView("~/Views/BasicData/GatherParamsSetting/_SelectGroupModal.cshtml", model);
        }
    }
}