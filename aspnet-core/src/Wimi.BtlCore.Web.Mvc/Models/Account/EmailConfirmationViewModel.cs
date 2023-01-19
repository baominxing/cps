using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.Web.Models.Account
{
    public class EmailConfirmationViewModel
    {
        [Required]
        public string ConfirmationCode { get; set; }

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