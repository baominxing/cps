using Abp.AutoMapper;

namespace Wimi.BtlCore.Order.Products.Dtos
{
    [AutoMap(typeof(Product))]
    public class ProductDto : ProductRequestDto
    {
        public bool Status { get; set; }
    }
}
