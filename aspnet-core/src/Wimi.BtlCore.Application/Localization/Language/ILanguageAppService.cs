using System.Threading.Tasks;

using Abp.Application.Services;
using Abp.Application.Services.Dto;

using Wimi.BtlCore.Localization.Dto;

namespace Wimi.BtlCore.Localization
{
    public interface ILanguageAppService : IApplicationService
    {
        Task CreateOrUpdateLanguage(CreateOrUpdateLanguageInputDto input);

        Task DeleteLanguage(EntityDto input);

        Task<GetLanguageForEditOutputDto> GetLanguageForEdit(NullableIdDto input);

        Task<GetLanguagesOutputDto> GetLanguages();

        Task<PagedResultDto<LanguageTextListDto>> GetLanguageTexts(GetLanguageTextsInputDto input);

        Task SetDefaultLanguage(SetDefaultLanguageInputDto input);

        Task UpdateLanguageText(UpdateLanguageTextInputDto input);
    }
}