using Abp.AutoMapper;
using Wimi.BtlCore.Order.Products.Dtos;

namespace Wimi.BtlCore.Web.Models.Order.Product
{
    [AutoMap(typeof(ProductGroupDto))]
    public class ProductGroupModel : ProductGroupDto
    {
        public ProductGroupModel()
        {
            this.IsEditMode = false;
        }

        public bool IsEditMode { get; set; }
    }
}
