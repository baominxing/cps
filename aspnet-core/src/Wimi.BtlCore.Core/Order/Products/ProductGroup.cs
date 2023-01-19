using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Order.Products
{
    /// <summary>
    /// 产品组
    /// </summary>
    [Table("ProductGroups")]
    public class ProductGroup : FullAuditedEntity
    {
        public ProductGroup()
        {
            this.Products = new HashSet<Product>();
        }

        /// <summary>
        /// 产品组代码
        /// </summary>
        [MaxLength(50)]
        [Comment("编码")]
        public string Code { get; set; }

        /// <summary>
        /// 产品组备注
        /// </summary>
        [MaxLength(200)]
        [Comment("描述")]
        public string Memo { get; set; }

        /// <summary>
        /// 产品组名称
        /// </summary>
        [MaxLength(50)]
        [Comment("名字")]
        public string Name { get; set; }

        /// <summary>
        /// 产品
        /// </summary>
        public virtual ICollection<Product> Products { get; set; }
    }
}
