using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Models.Users;
using System.Web;

namespace Wimi.BtlCore.Web.Controllers
{
    [AbpMvcAuthorize(PermissionNames.Pages_Administration_Users)]
    public class UsersController : BtlCoreControllerBase
    {
        private readonly IUserAppService userAppService;

        private readonly IUserLoginAppService userLoginAppService;

        private readonly UserManager userManager;

        public UsersController(
            IUserAppService userAppService,
            UserManager userManager,
            IUserLoginAppService userLoginAppService)
        {
            this.userAppService = userAppService;
            this.userManager = userManager;
            this.userLoginAppService = userLoginAppService;
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Administration_Users_Create,
            PermissionNames.Pages_Administration_Users_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(long? id)
        {
            var output = await this.userAppService.GetUserForEdit(new NullableIdDto<long> { Id = id });
            var viewModel = new CreateOrEditUserModalViewModel(output);

            return this.PartialView("Views/App/Users/_CreateOrEditModal.cshtml", viewModel);
        }

        public async Task<PartialViewResult> ResetPasswordModal(long? id)
        {
            var output = await this.userAppService.GetUserForEdit(new NullableIdDto<long> { Id = id });
            var viewModel = new CreateOrEditUserModalViewModel(output);
            return PartialView("Views/App/Users/_ResetPasswordModal.cshtml", viewModel);
        }

        public ActionResult Index()
        {
            var model = new UsersViewModel { FilterText = this.Request.Query["filterText"] };

            return this.View("Views/App/Users/Index.cshtml", model);
        }

        public async Task<PartialViewResult> LoginAttemptsModal()
        {
            var output = await this.userLoginAppService.GetRecentUserLoginAttempts();
            var model = new UserLoginAttemptModalViewModel { LoginAttempts = output.Items.ToList() };
            return this.PartialView("Views/App/Users/_LoginAttemptsModal.cshtml", model);
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Administration_Users_ChangePermissions)]
        public async Task<PartialViewResult> PermissionsModal(long id)
        {
            var user = await this.userManager.GetUserByIdAsync(id);
            var output = await this.userAppService.GetUserPermissionsForEdit(new EntityDto<long>(id));
            var viewModel = new UserPermissionsEditViewModel(output, user);

            return this.PartialView("Views/App/Users/_PermissionsModal.cshtml", viewModel);
        }
    }
}
