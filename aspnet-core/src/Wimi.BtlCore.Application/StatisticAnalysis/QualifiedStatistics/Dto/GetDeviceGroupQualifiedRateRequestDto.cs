using System.Collections.Generic;
using Wimi.BtlCore.CommonEnums;

namespace Wimi.BtlCore.StatisticAnalysis.QualifiedStatistics.Dto
{
    public class GetDeviceGroupQualifiedRateRequestDto
    {
        public string EndTime { get; set; }

        // 设备Id列表
        public List<int> DeviceGroupIdList { get; set; }

        public List<int> ShiftSolutionIdList { get; set; }

        // 时间区间
        public string StartTime { get; set; }

        public EnumStatisticalWays StatisticalWay { get; set; }

        public string SummaryDate { get; set; }
        public List<string> UnionTables { get; internal set; } = new List<string>();
    }
}
