using System;
using System.Collections.Generic;

namespace Wimi.BtlCore.StatisticAnalysis.Yield.Dto
{
    public class QualificationRateDto
    {
        public QualificationRateDto()
        {

        }

        public QualificationRateDto(int deviceGroupId, string deviceGroupName)
        {
            this.DeviceGroupId = deviceGroupId;
            this.DeviceGroupName = deviceGroupName;
        }

        public string DeviceGroupName { get; set; }

        public int DeviceGroupId { get; set; }

        public List<OKRateChartData> ChartDataList { get; set; } = new List<OKRateChartData>();

        public List<QualificationRateItem> Items { get; set; } = new List<QualificationRateItem>();

    }

    public class QualificationRateItem
    {
        public DateTime ShiftDayTime { get; set; }

        public string ShiftDay => this.ShiftDayTime.ToString("yyyy-MM-dd");

       // public decimal OkRate => this.Count == 0 ? 0 : 100 - Math.Round(this.UnqualifiedCount / this.Count, 4) * 100;

        public decimal Count { get; set; }

        public int UnqualifiedCount { get; set; }

        public string DeviceGroupName { get; set; }

        public int DeviceGroupId { get; set; }

        public string SummaryDate { get; set; }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public decimal OkRate { get; set; }

        public string SummaryDateOrder { get; set; }

        public int ItemIdOrder { get; set; }
    }

    public class OKRateChartData
    {
        public string SummaryDate { get; set; }

        public string MachineName { get; set; }

        public string ProgramName { get; set; }

        public List<decimal> OKRates { get; set; } = new List<decimal>();

        public List<int> UnqualifiedCounts { get; set; } = new List<int>();

        public string SummaryDateOrder { get; set; }

        public int ItemIdOrder { get; set; }
    }
}