using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Editions;
using Wimi.BtlCore.Editions.Dto;
using Wimi.BtlCore.Web.Models.App;

namespace Wimi.BtlCore.Web.Controllers.App
{
    [AbpMvcAuthorize(PermissionNames.Pages_Editions)]
    public class EditionsController : BtlCoreControllerBase
    {
        private readonly IEditionAppService editionAppService;

        public EditionsController(IEditionAppService editionAppService)
        {
            this.editionAppService = editionAppService;
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Editions_Create, PermissionNames.Pages_Editions_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(int? id)
        {
            var output = await this.editionAppService.GetEditionForEdit(new NullableIdDto { Id = id });
            var viewModel = new CreateOrEditEditionModalViewModel(output);

            return this.PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<JsonResult> GetEditions(GetEditionsInputDto input)
        {
            var edition = await this.editionAppService.GetEditions(input);
            return this.Json(edition);
        }

        public ActionResult Index()
        {
            return this.View();
        }
    }
}
