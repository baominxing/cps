using System.ComponentModel.DataAnnotations;
using Abp.Auditing;

namespace Wimi.BtlCore.Authorization.Users.Dto
{
    public class LinkToUserInputDto
    {
        public string TenancyName { get; set; }

        [Required]
        public string UsernameOrEmailAddress { get; set; }

        [Required]
        [DisableAuditing]
        public string Password { get; set; }
    }
}