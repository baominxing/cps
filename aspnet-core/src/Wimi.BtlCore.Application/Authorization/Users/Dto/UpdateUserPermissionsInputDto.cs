using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.Authorization.Users.Dto
{
    public class UpdateUserPermissionsInputDto
    {
        [Required]
        public List<string> GrantedPermissionNames { get; set; }

        [Range(1, int.MaxValue)]
        public long Id { get; set; }
    }
}
