namespace Wimi.BtlCore.OEE
{
    using Abp.Application.Services.Dto;

    public class OeeResponse
    {
        public decimal PlanedWorkingTime { get; set; }

        public decimal ActualWorkTime { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public decimal PerfectTime { get; set; }

        public decimal ActualTime { get; set; }

        public decimal TotalDuration { get; set; }

        public decimal ReasonDuration { get; set; }

        public decimal TotalYiled { get; set; }

        public decimal QualifiedCount => this.TotalCount - this.UnqualifiedCount;

        public decimal UnqualifiedCount { get; set; }

        public decimal TotalCount { get; set; }

        public string ShiftDay { get; set; }

        public string Date { get; set; }

        /// <summary>
        /// 可用率
        /// </summary>
        public string Availability { get; set; }

        /// <summary>
        /// 质量指数
        /// </summary>
        public string QualityIndicators { get; set; }

        public decimal Rate { get; set; }

        /// <summary>
        /// 表现指数
        /// </summary>
        public NameValueDto<decimal> PerformanceIndicators { get; set; }
    }
}