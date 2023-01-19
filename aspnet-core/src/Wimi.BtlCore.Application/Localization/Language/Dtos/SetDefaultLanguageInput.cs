using System.ComponentModel.DataAnnotations;

using Abp.Localization;

namespace Wimi.BtlCore.Localization.Dto
{
    public class SetDefaultLanguageInputDto
    {
        [Required]
        [StringLength(ApplicationLanguage.MaxNameLength)]
        public virtual string Name { get; set; }
    }
}