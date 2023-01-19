using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Order.Products
{
    /// <summary>
    /// 产品
    /// </summary>
    [Table("Products")]
    public class Product : FullAuditedEntity
    {
        /// <summary>
        /// 产品组代码
        /// </summary>
        [Comment("编码")]
        [MaxLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// 产品组描述
        /// </summary>
        [Comment("描述")]
        [MaxLength(200)]
        public string Desc { get; set; }

        /// <summary>
        /// 是否是半成品
        /// </summary>
        [Comment("是否半成品")]
        [MaxLength(10)]
        public string IsHalfFinished { get; set; }

        /// <summary>
        /// 产品组备注
        /// </summary>
        [Comment("备注")]
        [MaxLength(200)]
        public string Memo { get; set; }

        /// <summary>
        /// 产品组名称
        /// </summary>
        [Comment("名称")]
        [MaxLength(50)]
        public string Name { get; set; }

        [ForeignKey("ProductGroupId")]
        public virtual ProductGroup ProductGroup { get; set; }

        /// <summary>
        /// 所属产品组Id
        /// </summary>
        [Comment("产品组key")]
        public int ProductGroupId { get; set; }

        /// <summary>
        /// 产品组规格
        /// </summary>
        [Comment("规格")]
        [MaxLength(50)]
        public string Spec { get; set; }

        /// <summary>
        /// 图纸号
        /// </summary>
        [Comment("图纸号")]
        [MaxLength(50)]
        public string DrawingNumber { get; set; }

        /// <summary>
        /// 零件类型
        /// </summary>
        [Comment("零件类型")]
        [MaxLength(50)]
        public string PartType { get; set; }

        public ProductNameValue ToProductNameValue()
        {
            var result = new ProductNameValue();
            result.Name = this.Name;
            result.Value = this.Id;

            return result;
        }
    }
}
