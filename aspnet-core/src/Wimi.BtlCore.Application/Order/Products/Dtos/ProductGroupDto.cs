using Abp.AutoMapper;

namespace Wimi.BtlCore.Order.Products.Dtos
{
    [AutoMap(typeof(ProductGroup))]
    public class ProductGroupDto : ProductRequestDto
    {
    }
}
