namespace Wimi.BtlCore.Configuration.Host.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class HostSettingsEditDto
    {
        [Required]
        public EmailSettingsEditDto Email { get; set; }

        [Required]
        public GeneralSettingsEditDto General { get; set; }

        [Required]
        public TenantManagementSettingsEditDto TenantManagement { get; set; }

        [Required]
        public HostUserManagementSettingsEditDto UserManagement { get; set; }
    }
}