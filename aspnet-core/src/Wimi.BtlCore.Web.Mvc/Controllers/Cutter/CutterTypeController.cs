using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Cutter;
using Wimi.BtlCore.Cutter.Dto;
using Wimi.BtlCore.Web.Models.Cutter.CutterType;

namespace Wimi.BtlCore.Web.Controllers.Cutter
{
    public class CutterTypeController : BtlCoreControllerBase
    {
        private readonly ICutterAppService cutterAppService;

        public CutterTypeController(ICutterAppService cutterAppService)
        {
            this.cutterAppService = cutterAppService;
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Cutter_CutterType)]
        public async Task<PartialViewResult> CreateOrUpdateModalForModel(int? id, int cutterTypeId)
        {
            var model = new CutterModelModel
            {
                CutterTypeId = cutterTypeId
            };

            var cutterTypeModel = await this.cutterAppService.GetCutterTypeForEdit(new CutterTypeDto() { Id = cutterTypeId });

            model.CutterNoPrefix = cutterTypeModel.CutterNoPrefix;
            model.IsCutterNoPrefixCanEdit = cutterTypeModel.IsCutterNoPrefixCanEdit;

            if (!id.HasValue || id == 0)
            {
                model.IsEditMode = false;
            }
            else
            {
                model = ObjectMapper.Map<CutterModelModel>(await this.cutterAppService.GetCutterModelForEdit(new CutterModelDto() { Id = (int)id }));
                model.IsEditMode = true;
            }
            model.ValidStatusList = GetValidStatusComboboxItemList(model);
            return this.PartialView("~/Views/Cutter/CutterType/_CreateOrUpdateModalForModel.cshtml", model);
        }

        private List<ComboboxItemDto> GetValidStatusComboboxItemList(CutterModelModel model)
        {
            var numberComboxboxItem = new ComboboxItemDto(EnumCountingMethod.Number.ToString(), L($"Cutter-{EnumCountingMethod.Number.ToString()}"));
            var timeComboxboxItem = new ComboboxItemDto(EnumCountingMethod.Time.ToString(), L($"Cutter-{EnumCountingMethod.Time.ToString()}"));
            var validStatus = new List<ComboboxItemDto>();

            validStatus.Add(numberComboxboxItem);
            validStatus.Add(timeComboxboxItem);

            if (!model.IsEditMode)
            {
                return validStatus;
            }

            foreach (var item in validStatus)
            {
                if (item.Value == model.CountingMethod.ToString())
                {
                    item.IsSelected = true;
                    break;
                }
            }

            return validStatus;
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Cutter_CutterType)]
        public async Task<PartialViewResult> CreateOrUpdateModalForType(int? id, bool isCreateFromContextMenu = false)
        {
            var model = new CutterTypeModel();

            if (isCreateFromContextMenu)
            {
                model.PId = id;
                model.IsCreateFromContextMenu = true;
            }
            else if (id.HasValue && id != 0)
            {
                model =
                    ObjectMapper.Map<CutterTypeModel>(
                        await this.cutterAppService.GetCutterTypeForEdit(new CutterTypeDto() { Id = (int)id }));
                model.IsEditMode = true;
            }

            return this.PartialView("~/Views/Cutter/CutterType/_CreateOrUpdateModalForType.cshtml", model);
        }

        // GET: ToolType
        public ActionResult Index()
        {
            return this.View("~/Views/Cutter/CutterType/Index.cshtml");
        }
    }
}
