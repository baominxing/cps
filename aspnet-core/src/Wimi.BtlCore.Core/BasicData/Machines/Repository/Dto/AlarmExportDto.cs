using System;

namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class AlarmExportDto
    {
        public string MachineCode { get; set; }

        public string MachineName { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public int Count { get; set; }

        public int TotalCount { get; set; }

        public string SummaryDate { get; set; }

        public double Rate => this.TotalCount == 0 ? 0 : Math.Round((this.Count * 1.0 / this.TotalCount), 4);

        public string RateName => this.Rate.ToString("P");
    }
}
