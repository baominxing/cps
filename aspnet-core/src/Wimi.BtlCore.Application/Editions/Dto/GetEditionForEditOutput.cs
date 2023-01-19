using System.Collections.Generic;

using Abp.Application.Services.Dto;

namespace Wimi.BtlCore.Editions.Dto
{
    public class GetEditionForEditOutputDto
    {
        public EditionEditDto Edition { get; set; }

        public List<FlatFeatureDto> Features { get; set; }

        public List<NameValueDto> FeatureValues { get; set; }
    }
}