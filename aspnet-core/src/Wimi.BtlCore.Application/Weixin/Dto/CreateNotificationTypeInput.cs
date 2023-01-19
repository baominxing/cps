namespace Wimi.BtlCore.Weixin.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class CreateNotificationTypeInputDto
    {
        [Required]
        [StringLength(128)]
        public string DisplayName { get; set; }

        public int? ParentId { get; set; }
    }
}