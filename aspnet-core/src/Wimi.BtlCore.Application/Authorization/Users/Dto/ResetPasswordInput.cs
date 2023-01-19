using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.Authorization.Users.Dto
{
    public class ResetPasswordInput
    {
        [Required]
        public UserResetPasswordDto User { get; set; }

    }
}