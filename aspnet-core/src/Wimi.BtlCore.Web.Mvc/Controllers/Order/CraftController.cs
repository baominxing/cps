namespace Wimi.BtlCore.Web.Controllers.Order
{
    using Abp.AspNetCore.Mvc.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.Controllers;
    using Wimi.BtlCore.Order.Crafts;
    using Wimi.BtlCore.Order.Crafts.Dtos;
    using Wimi.BtlCore.Web.Models.Order.Craft;

    public class CraftController : BtlCoreControllerBase
    {
        private readonly ICraftAppService craftAppService;

        public CraftController(ICraftAppService craftAppService)
        {
            this.craftAppService = craftAppService;
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Order_Craft_Manage)]
        public async Task<PartialViewResult> CreateOrUpdateCraft(int? id)
        {
            var model = new CraftModel();
            if (id.HasValue && id != 0)
            {
                var query = await this.craftAppService.GetCraftForEdit(new CraftRequestDto() { Id = (int)id });
                model = ObjectMapper.Map<CraftModel>(query);
                model.IsEditMode = true;
            }

            return this.PartialView("~/Views/Orders/Craft/_CreateOrUpdateCraft.cshtml", model);
        }

        // GET: Product
        public ActionResult Index()
        {
            return this.View("~/Views/Orders/Craft/Index.cshtml");
        }
    }
}