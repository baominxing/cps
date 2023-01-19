using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Authorization.Roles;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Models.Roles;

namespace Wimi.BtlCore.Web.Controllers
{
    [AbpMvcAuthorize(PermissionNames.Pages_Administration_Roles)]
    public class RolesController : BtlCoreControllerBase
    {
        private readonly IRoleAppService roleAppService;

        public RolesController(IRoleAppService roleAppService)
        {
            this.roleAppService = roleAppService;
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Administration_Roles_Create,
            PermissionNames.Pages_Administration_Roles_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(int? id)
        {
            var output = await this.roleAppService.GetRoleForEdit(new NullableIdDto { Id = id });
            var viewModel = new CreateOrEditRoleModalViewModel(output);

            return this.PartialView("Views/App/Roles/_CreateOrEditModal.cshtml", viewModel);
        }

        public ActionResult Index()
        {
            return this.View("Views/App/Roles/Index.cshtml");
        }
    }
}
