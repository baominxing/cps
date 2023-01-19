using Abp.AutoMapper;
using Wimi.BtlCore.Editions.Dto;
using Wimi.BtlCore.Web.Models.Common;

namespace Wimi.BtlCore.Web.Models.App
{
    [AutoMapFrom(typeof(GetEditionForEditOutputDto))]
    public class CreateOrEditEditionModalViewModel : GetEditionForEditOutputDto, IFeatureEditViewModel
    {
        public CreateOrEditEditionModalViewModel(GetEditionForEditOutputDto output)
        {
            Edition = output.Edition;
            Features = output.Features;
            FeatureValues = output.FeatureValues;
        }

        public bool IsEditMode
        {
            get { return this.Edition.Id.HasValue; }
        }
    }
}
