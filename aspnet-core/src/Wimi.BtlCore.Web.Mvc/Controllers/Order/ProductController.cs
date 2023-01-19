using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Order.Product;
using Wimi.BtlCore.Order.Products.Dtos;
using Wimi.BtlCore.Web.Models.Order.Product;

namespace Wimi.BtlCore.Web.Controllers.Order
{
    public class ProductController : BtlCoreControllerBase
    {
        private readonly IProductAppService productAppService;

        public ProductController(IProductAppService productAppService)
        {
            this.productAppService = productAppService;
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Order_Product_Manage)]
        public async Task<PartialViewResult> CreateOrUpdateProduct(int? id, int productGroupId)
        {
            var model = new ProductModel
            {
                ProductGroupId = productGroupId
            };

            if (!id.HasValue || id == 0) return this.PartialView("~/Views/Orders/Product/_CreateOrUpdateProduct.cshtml", model);

            var query = await this.productAppService.GetProductForEdit(new ProductRequestDto() { Id = (int)id });
            model = ObjectMapper.Map<ProductModel>(query);
            model.IsEditMode = true;

            return this.PartialView("~/Views/Orders/Product/_CreateOrUpdateProduct.cshtml", model);
        }

        [AbpMvcAuthorize(PermissionNames.Pages_Order_Product_Manage)]
        public async Task<PartialViewResult> CreateOrUpdateProductGroup(int? id)
        {
            var model = new ProductGroupModel();
            if (id.HasValue && id != 0)
            {
                var query = await this.productAppService.GetProductGroupForEdit(new ProductGroupRequestDto() { Id = (int)id });

                model = ObjectMapper.Map<ProductGroupModel>(query);
                model.IsEditMode = true;
            }

            return this.PartialView("~/Views/Orders/Product/_CreateOrUpdateProductGroup.cshtml", model);
        }

        // GET: Product
        public ActionResult Index()
        {
            return this.View("~/Views/Orders/Product/Index.cshtml");
        }
    }
}