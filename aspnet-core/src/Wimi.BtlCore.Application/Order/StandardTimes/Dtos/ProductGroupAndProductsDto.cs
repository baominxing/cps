using Abp.Application.Services.Dto;

namespace Wimi.BtlCore.Order.StandardTimes.Dtos
{
    public class ProductGroupAndProductsDto
    {
        public ProductGroupAndProductsDto()
        {
            this.Product = new NameValueDto<int>[] { };
        }

        public string ProductGroupName { get; set; }

        public NameValueDto<int>[] Product { get; set; }
    }
}
