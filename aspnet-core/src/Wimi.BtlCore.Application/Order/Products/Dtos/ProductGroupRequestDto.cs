using Abp.AutoMapper;
using System.Collections.Generic;

namespace Wimi.BtlCore.Order.Products.Dtos
{
    [AutoMap(typeof(ProductGroup))]
    public class ProductGroupRequestDto
    {
        public ProductGroupRequestDto()
        {
            this.ProductIdList = new List<int>();
        }

        public string Code { get; set; }

        public int Id { get; set; }

        public string Memo { get; set; }

        public string Name { get; set; }

        public List<int> ProductIdList { get; set; }
    }
}
