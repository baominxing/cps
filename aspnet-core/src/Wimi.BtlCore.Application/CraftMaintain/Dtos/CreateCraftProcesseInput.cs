using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.CraftMaintain.Dtos
{
    [AutoMap(typeof(FlexibleCraftProcesse))]
    public class CreateCraftProcesseInput : NullableIdDto
    {
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 夹具Id
        /// </summary>
        public int TongId { get; set; }
    }
}
