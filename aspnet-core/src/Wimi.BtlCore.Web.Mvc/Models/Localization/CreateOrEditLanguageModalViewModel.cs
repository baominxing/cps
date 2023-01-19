using Abp.AutoMapper;
using Wimi.BtlCore.Localization.Dto;

namespace Wimi.BtlCore.Web.Models.Localization
{
    [AutoMapFrom(typeof(GetLanguageForEditOutputDto))]
    public class CreateOrEditLanguageModalViewModel : GetLanguageForEditOutputDto
    {
        public CreateOrEditLanguageModalViewModel(GetLanguageForEditOutputDto output)
        {
            Flags = output.Flags;
            Language = output.Language;
            LanguageNames = output.LanguageNames;
        }

        public bool IsEditMode
        {
            get
            {
                return this.Language.Id.HasValue;
            }
        }
    }
}
