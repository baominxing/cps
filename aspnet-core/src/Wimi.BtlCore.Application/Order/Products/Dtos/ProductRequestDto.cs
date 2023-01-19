using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace Wimi.BtlCore.Order.Products.Dtos
{
    [AutoMap(typeof(Product))]
    public class ProductRequestDto
    {
        public ProductRequestDto()
        {
            this.ProductCraftIdList = new List<int>();
        }

        public string Code { get; set; }

        public string Desc { get; set; }

        public int Id { get; set; }

        public string IsHalfFinished { get; set; }

        public string Memo { get; set; }

        public string Name { get; set; }

        public List<int> ProductCraftIdList { get; set; }

        public int ProductGroupId { get; set; }

        public string Spec { get; set; }

        /// <summary>
        /// 图纸号
        /// </summary>
        [MaxLength(50)]
        public string DrawingNumber { get; set; }

        /// <summary>
        /// 零件类型
        /// </summary>
        [MaxLength(50)]
        public string PartType { get; set; }
    }
}
