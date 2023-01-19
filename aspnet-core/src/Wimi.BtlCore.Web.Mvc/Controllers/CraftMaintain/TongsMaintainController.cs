using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Tongs;
using Wimi.BtlCore.Tongs.Dto;
using Wimi.BtlCore.Web.Models.CraftMaintain;

namespace Wimi.BtlCore.Web.Controllers.CraftMaintain
{
    [AbpMvcAuthorize(PermissionNames.Pages_CraftMaintain_TongsMaintain)]
    public class TongsMaintainController : BtlCoreControllerBase
    {
        // GET: Process
        private readonly ITongAppService tongAppService;

        public TongsMaintainController(ITongAppService tongAppService)
        {
            this.tongAppService = tongAppService;
        }

        public ActionResult Index()
        {
            return this.View("~/Views/CraftMaintain/TongsMaintain/Index.cshtml");
        }

        public async Task<PartialViewResult> CreateOrUpdateModal(int? id)
        {
            var model = new TongViewModel();

            if (id.HasValue)
            {
                var dto = await this.tongAppService.GetTongForEdit(new TongInputDto() { Id = id.Value });

                if (dto != null)
                {
                    model.Dto = dto;
                    model.IsEditMode = true;
                }
            }

            return this.PartialView("~/Views/CraftMaintain/TongsMaintain/CreateOrUpdateModal.cshtml", model);
        }
    }
}