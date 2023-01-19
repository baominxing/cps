using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Wimi.BtlCore.Order.StandardTimes.Dtos
{
    /// <summary>
    /// 标准用时Dto
    /// </summary>
    [AutoMap(typeof(StandardTime))]
    public class StandardTimeDto
    {
        public StandardTimeDto()
        {
            this.Process = new NameValueDto<int>[] { };
            this.ProductGroupAndProductsDto = new ProductGroupAndProductsDto[] { };
        }

        public int CycleRate { get; set; }

        public int Id { get; set; }

        public string Memo { get; set; }

        public string ProcessCode { get; set; }

        public int ProcessId { get; set; }

        public string ProcessName { get; set; }

        public string ProductCode { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal StandardCostTime { get; set; }

        public NameValueDto<int>[] Process { get; set; }

        public ProductGroupAndProductsDto[] ProductGroupAndProductsDto { get; set; }
    }
}
