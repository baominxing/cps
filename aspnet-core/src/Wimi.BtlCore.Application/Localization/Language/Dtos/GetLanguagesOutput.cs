using System.Collections.Generic;

using Abp.Application.Services.Dto;

namespace Wimi.BtlCore.Localization.Dto
{
    public class GetLanguagesOutputDto : ListResultDto<ApplicationLanguageListDto>
    {
        public GetLanguagesOutputDto()
        {
        }

        public GetLanguagesOutputDto(IReadOnlyList<ApplicationLanguageListDto> items, string defaultLanguageName)
            : base(items)
        {
            this.DefaultLanguageName = defaultLanguageName;
        }

        public string DefaultLanguageName { get; set; }
    }
}