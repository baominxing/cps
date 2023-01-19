using System.Collections.Generic;

namespace Wimi.BtlCore.StatisticAnalysis.QualifiedStatistics.Dto
{
    public class ListQualificationInfoResultDto
    {
        public List<string> SumarryDateList { get; set; } = new List<string>();

        public IEnumerable<QualifiedStatisticsDto> TableData { get; set; }
    }
}
