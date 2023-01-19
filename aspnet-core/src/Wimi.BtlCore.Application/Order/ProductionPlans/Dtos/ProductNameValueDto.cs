using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Wimi.BtlCore.Order.Products;

namespace Wimi.BtlCore.Order.ProductionPlans.Dtos
{
    [AutoMapFrom(typeof(ProductNameValue))]
    public class ProductNameValueDto : NameValueDto<int>
    {
    }
}
