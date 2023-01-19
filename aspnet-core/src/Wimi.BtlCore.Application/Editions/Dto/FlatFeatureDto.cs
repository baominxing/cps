using Abp.Application.Features;
using Abp.AutoMapper;
using Abp.UI.Inputs;

namespace Wimi.BtlCore.Editions.Dto
{
    [AutoMapFrom(typeof(Feature))]
    public class FlatFeatureDto
    {
        public string DefaultValue { get; set; }

        public string Description { get; set; }

        public string DisplayName { get; set; }

        public IInputType InputType { get; set; }

        public string Name { get; set; }

        public string ParentName { get; set; }
    }
}