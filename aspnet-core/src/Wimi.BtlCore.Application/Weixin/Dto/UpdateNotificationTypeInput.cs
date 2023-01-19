namespace Wimi.BtlCore.Weixin.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateNotificationTypeInputDto
    {
        [Required]
        [StringLength(128)]
        public string DisplayName { get; set; }

        [Range(1, int.MaxValue)]
        public int Id { get; set; }
    }
}