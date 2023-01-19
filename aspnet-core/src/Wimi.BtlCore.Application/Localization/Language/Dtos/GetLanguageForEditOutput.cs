using System.Collections.Generic;

using Abp.Application.Services.Dto;

namespace Wimi.BtlCore.Localization.Dto
{
    public class GetLanguageForEditOutputDto
    {
        public GetLanguageForEditOutputDto()
        {
            this.LanguageNames = new List<ComboboxItemDto>();
            this.Flags = new List<ComboboxItemDto>();
        }

        public List<ComboboxItemDto> Flags { get; set; }

        public ApplicationLanguageEditDto Language { get; set; }

        public List<ComboboxItemDto> LanguageNames { get; set; }
    }
}