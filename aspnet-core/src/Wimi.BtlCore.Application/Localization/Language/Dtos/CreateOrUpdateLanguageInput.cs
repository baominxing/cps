using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.Localization.Dto
{
    public class CreateOrUpdateLanguageInputDto
    {
        [Required]
        public ApplicationLanguageEditDto Language { get; set; }
    }
}