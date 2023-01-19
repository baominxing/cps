using Abp.AutoMapper;
using Wimi.BtlCore.Order.Products.Dtos;

namespace Wimi.BtlCore.Web.Models.Order.Product
{
    [AutoMap(typeof(ProductDto))]
    public class ProductModel : ProductDto
    {
        public ProductModel()
        {
            this.IsEditMode = false;
        }

        public bool IsEditMode { get; set; }
    }
}
