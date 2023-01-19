using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.Authorization.Users.Dto
{
    public class CreateOrUpdateUserInputDto
    {
        [Required]
        public UserEditDto User { get; set; }

        [Required]
        public string[] AssignedRoleNames { get; set; }

        public bool SendActivationEmail { get; set; }

        public bool SetRandomPassword { get; set; }

    }
}