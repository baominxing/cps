using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.Authorization.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}
