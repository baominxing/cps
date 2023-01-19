using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.Web.Models.Account
{
    public class ResetPasswordViewModel
    {
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