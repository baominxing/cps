namespace Wimi.BtlCore.Visual.Dto
{
    using System.ComponentModel.DataAnnotations;

    using Abp.Application.Services.Dto;
    
    public class GetWorkShopsDto : EntityDto
    {
        /// <summary>
        /// 车间编码
        /// </summary>
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Code { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        [Required]
        public int MachineGroupId { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Name { get; set; }
    }
}