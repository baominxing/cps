namespace Wimi.BtlCore.Visual.Dto
{
    using System.ComponentModel.DataAnnotations;

    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;
    using Wimi.BtlCore.Notices;

    [AutoMap(typeof(Notice))]
    public class GetNoticeOutputDto : AuditedEntityDto<int?>
    {
        [Required]
        [MaxLength(BtlCoreConsts.MaxDescLength * 5)]
        public string Content { get; set; }

        public string CreatorUserName { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string WorkShopCode { get; set; }

        public string WorkShopName { get; set; }
    }
}