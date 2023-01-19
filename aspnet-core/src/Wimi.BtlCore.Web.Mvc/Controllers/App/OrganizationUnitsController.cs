using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Organizations;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Web.Models.App;

namespace Wimi.BtlCore.Web.Controllers.App
{
    [AbpMvcAuthorize(PermissionNames.Pages_Administration_OrganizationUnits)]
    public class OrganizationUnitsController : BtlCoreControllerBase
    {
        private readonly IRepository<OrganizationUnit, long> organizationUnitRepository;

        public OrganizationUnitsController(IRepository<OrganizationUnit, long> organizationUnitRepository)
        {
            this.organizationUnitRepository = organizationUnitRepository;
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Administration_OrganizationUnits_ManageOrganizationTree)]
        public PartialViewResult CreateModal(long? parentId)
        {
            return this.PartialView("_CreateModal", new CreateOrganizationUnitModalViewModel(parentId));
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Administration_OrganizationUnits_ManageOrganizationTree)]
        public async Task<PartialViewResult> EditModal(long id)
        {
            var organizationUnit = await this.organizationUnitRepository.GetAsync(id);
            var model = ObjectMapper.Map<EditOrganizationUnitModalViewModel>(organizationUnit);

            return this.PartialView("_EditModal", model);
        }

        public ActionResult Index()
        {
            return this.View();
        }
    }
}
