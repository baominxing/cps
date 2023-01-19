using Abp.Extensions;
using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wimi.BtlCore.Configuration.Host.Dto;

namespace Wimi.BtlCore.Configuration.Tenants.Dto
{
    public class TenantSettingsEditDto
    {
        public EmailSettingsEditDto Email { get; set; }

        public GeneralSettingsEditDto General { get; set; }

        public LdapSettingsEditDto Ldap { get; set; }

        [Required]
        public TenantUserManagementSettingsEditDto UserManagement { get; set; }

        public VersionSettingsEditDto Version { get; set; }

        public CutterSettingEditDto Cutter { get; set; }

        /// <summary>
        ///     This validation is done for single-tenant applications.
        ///     Because, these settings can only be set by tenant in a single-tenant application.
        /// </summary>
        public void ValidateHostSettings()
        {
            var validationErrors = new List<ValidationResult>();
            if (this.General == null)
            {
                validationErrors.Add(new ValidationResult("General settings can not be null", new[] { "General" }));
            }
            else
            {
                if (this.General.WebSiteRootAddress.IsNullOrEmpty())
                {
                    validationErrors.Add(
                        new ValidationResult(
                            "General.WebSiteRootAddress can not be null or empty", 
                            new[] { "WebSiteRootAddress" }));
                }
            }

            if (this.Email == null)
            {
                validationErrors.Add(new ValidationResult("Email settings can not be null", new[] { "Email" }));
            }

            if (validationErrors.Count > 0)
            {
                throw new AbpValidationException(
                    "Method arguments are not valid! See ValidationErrors for details.", 
                    validationErrors);
            }
        }
    }
}