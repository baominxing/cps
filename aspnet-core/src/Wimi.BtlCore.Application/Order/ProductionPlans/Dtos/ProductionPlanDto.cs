using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore.Order.ProductionPlans.Dtos
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;

    [AutoMap(typeof(ProductionPlan))]
    public class ProductionPlanDto : EntityDto<int?>
    {
        public ProductionPlanDto()
        {
            this.StartDate = DateTime.Now.Date;
            this.EndDate = DateTime.Now.Date;
        }

        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "目标量必须是正整数")]
        public int AimVolume { get; set; }

        [MaxLength(50)]
        public string ClientName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        public int CraftId { get; set; }

        public string CraftName { get; set; }

        public DateTime EndDate { get; set; }

        [MaxLength(50)]
        public string OrderCode { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }
        public string ProductCode { get; set; }

        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "投放量必须是正整数")]
        public int PutVolume { get; set; }

        public DateTime StartDate { get; set; }

        [MaxLength(10)]
        public string Unit { get; set; }

        public string Memo { get; set; }
    }
}
