namespace Wimi.BtlCore.Authorization.Roles.Dto
{
    using System.ComponentModel.DataAnnotations;

    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;
    using Wimi.BtlCore.Authorization.Roles;

    [AutoMap(typeof(Role))]
    public class RoleEditDto : EntityDto<int?>
    {
        [Required]
        public string DisplayName { get; set; }

        public bool IsDefault { get; set; }
    }
}