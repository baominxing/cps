using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.Web.Models.Account
{
    public class SendEmailActivationLinkViewModel
    {
        [Required]
        public string EmailAddress { get; set; }

        public string TenancyName { get; set; }
    }
}