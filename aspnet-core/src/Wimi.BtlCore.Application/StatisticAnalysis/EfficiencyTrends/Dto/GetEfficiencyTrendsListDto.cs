using Abp.Application.Services.Dto;
using System.Collections.Generic;
using Wimi.BtlCore.EfficiencyTrendas.Dtos;

namespace Wimi.BtlCore.StatisticAnalysis.EfficiencyTrends.Dto
{
    public class GetEfficiencyTrendsListDto
    {
        public List<EfficiencyTrendasDataTablesDto> EfficiencyTrendsColumns { get; set; }

        public ListResultDto<EfficiencyTrendasDataTablesDataDto> EfficiencyTrendsData { get; set; }

        public string GroupTypeName { get; set; }
    }
}
