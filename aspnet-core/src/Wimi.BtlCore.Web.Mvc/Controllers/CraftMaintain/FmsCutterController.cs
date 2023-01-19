using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.FmsCutters;
using Wimi.BtlCore.Controllers;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.CraftMaintain;
using Wimi.BtlCore.Web.Models.CraftMaintain;
using Wimi.BtlCore.FmsCutter.Dto;
using Wimi.BtlCore.CustomFields.Dto;

namespace Wimi.BtlCore.Web.Controllers.CraftMaintain
{
    [AbpMvcAuthorize(PermissionNames.Pages_CraftMaintain_FmsCutter)]
    public class FmsCutterController : BtlCoreControllerBase
    {
        // GET: Process
        private readonly IFmsCutterAppService fmscutterAppService;
        private readonly IRepository<CustomField> customFieldRepository;


        public FmsCutterController(IFmsCutterAppService fmscutterAppService, IRepository<CustomField> customFieldRepository)
        {
            this.fmscutterAppService = fmscutterAppService;
            this.customFieldRepository = customFieldRepository;
        }

        public ActionResult Index()
        {
            return this.View("~/Views/CraftMaintain/FmsCutter/Index.cshtml");
        }

        public async Task<PartialViewResult> CreateOrUpdateModal(int? id)
        {
            var model = new FmsCutterViewModel();

            if (id.HasValue)
            {
                var dto = await this.fmscutterAppService.GetFmsCutterForEdit(new FmsCutterInputDto() { Id = id.Value });

                if (dto != null)
                {
                    model.Dto = dto;
                    model.IsEditMode = true;
                }
            }

            model.Field = await this.fmscutterAppService.ListCutomFields();
            return this.PartialView("~/Views/CraftMaintain/FmsCutter/CreateOrUpdateModal.cshtml", model);
        }

        public async Task<PartialViewResult>  CreateExtendFieldModal(string code)
        {
            var model = new FmsCutterSettingViewModel();
            if (!code.IsNullOrEmpty())
            {
                model.IsEdit = true;
                var entity = await this.customFieldRepository.FirstOrDefaultAsync(t=>t.Code.Equals(code));
                if (entity != null)
                {
                    model.Field = ObjectMapper.Map<CustomFieldDto>(entity);
                }
            }
            return this.PartialView("~/Views/CraftMaintain/FmsCutter/_CreateExtendFieldModal.cshtml", model);
        }
    }
}