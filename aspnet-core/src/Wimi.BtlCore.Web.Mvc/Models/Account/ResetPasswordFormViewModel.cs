using System.ComponentModel.DataAnnotations;

using Abp.Auditing;

namespace Wimi.BtlCore.Web.Models.Account
{
    public class ResetPasswordFormViewModel
    {
        [Required]
        [DisableAuditing]
        public string Password { get; set; }

        [Required]
        public string ResetCode { get; set; }

        /// <summary>
        ///     Encrypted tenant id.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        ///     Encrypted user id.
        /// </summary>
        [Required]
        public string UserId { get; set; }
    }
}