using System.Collections.Generic;

using Abp.Application.Services.Dto;
using Wimi.BtlCore.Editions.Dto;

namespace Wimi.BtlCore.Web.Models.Common
{
    public interface IFeatureEditViewModel
    {
        List<FlatFeatureDto> Features { get; set; }

        List<NameValueDto> FeatureValues { get; set; }
    }
}
