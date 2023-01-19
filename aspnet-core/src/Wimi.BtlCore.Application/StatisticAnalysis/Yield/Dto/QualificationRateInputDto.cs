using System;
using System.Collections.Generic;
using Wimi.BtlCore.CommonEnums;

namespace Wimi.BtlCore.StatisticAnalysis.Yield.Dto
{
    public class QualificationInputRateDto
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public List<int> GroupIdList { get; set; }

        public EnumQueryMethod QueryMethod { get; set; }

        public EnumStatisticalWays StatisticalWay { get; set; }
    }
}