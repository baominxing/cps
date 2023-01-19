using System;

using Newtonsoft.Json;

namespace Wimi.BtlCore.Order.MachineReport.Dtos
{
    public class MachineReportDefectiveReasonDto
    {
        public string ShiftName { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public decimal? Yield { get; set; }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal? UnqualifiedCount { get; set; }

        public DateTime ShiftDay { get; set; }

        [JsonIgnore]
        public DateTime? StarTime { get; set; }

        public DateTime? CreationTime { get; set; }

        public decimal QualifiedCount => this.Yield.HasValue && this.UnqualifiedCount.HasValue
                                             ? (this.Yield.Value - this.UnqualifiedCount.Value)
                                             : 0;

        public decimal BadRate => this.Yield > 0 && this.UnqualifiedCount.HasValue
                                      ? Math.Round(this.UnqualifiedCount.Value / this.Yield.Value, 4)
                                      : 0;
    }
}
