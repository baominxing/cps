namespace Wimi.BtlCore.StatisticAnalysis.OEE.Dto
{
    using System;
    using System.Collections.Generic;

    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;
    using Wimi.BtlCore.OEE;

    [AutoMap(typeof(OeeResponse))]
    public class QualityStatusDto
    {
        public int MachineId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ShiftDay { get; set; }

        public decimal QualifiedCount => this.TotalCount - this.UnqualifiedCount;

        public decimal UnqualifiedCount { get; set; }

        public decimal TotalCount { get; set; }

        public decimal Rate => this.QualifiedCount == 0 && this.UnqualifiedCount == 0
                                   ? 1
                                   : Math.Round(this.QualifiedCount  / this.TotalCount, 4);

        public IEnumerable<NameValueDto<decimal>> ChartDto { get; set; }
    }
}