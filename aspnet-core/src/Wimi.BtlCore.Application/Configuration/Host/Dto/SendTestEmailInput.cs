namespace Wimi.BtlCore.Configuration.Host.Dto
{
    using System.ComponentModel.DataAnnotations;

    using Abp.Authorization.Users;

    public class SendTestEmailInputDto
    {
        [Required]
        [MaxLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
    }
}