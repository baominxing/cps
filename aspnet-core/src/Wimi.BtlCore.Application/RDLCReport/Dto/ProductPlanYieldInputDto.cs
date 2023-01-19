using System;
using Wimi.BtlCore.CommonEnums;

namespace Wimi.BtlCore.RDLCReport.Dto
{
    public class ProductPlanYieldInputDto
    {
        public DateTime EndTime { get; set; }

        public DateTime StartTime { get; set; }

        public EnumStatisticalWays StatisticalWay { get; set; }
    }
}
