using System.Collections.Generic;
using Wimi.BtlCore.QualifiedStatistics;

namespace Wimi.BtlCore.StatisticAnalysis.QualifiedStatistics.Dto
{
    public class DeviceGroupQualifiedRateDto
    {
        public IEnumerable<QualifiedStatisticsDto> TableDatas { get; set; } = new List<QualifiedStatisticsDto>();

        public IEnumerable<QualificationData> ChartDataList { get; set; } = new List<QualificationData>();

        public List<QualificationData> TableDataList { get; set; } = new List<QualificationData>();
    }
}
