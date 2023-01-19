using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.Collections.Generic;
using Wimi.BtlCore.FmsCutters;

namespace Wimi.BtlCore.CraftMaintain.Dtos
{
    public class CraftPathCutterDto
    {
        /// <summary>
        /// 工序Id
        /// </summary>
        public int CraftProcesseId { get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        public string CraftProcesseName { get; set; }

        /// <summary>
        /// 程序号
        /// </summary>
        public string ProcedureNumber { get; set; }

        /// <summary>
        /// 关联的刀具集合
        /// </summary>
        public List<GetCraftPathForCutterDetailDto> CutterDetails { get; set; }
         = new List<GetCraftPathForCutterDetailDto>();
    }

    [AutoMap(typeof(FmsCutters.FmsCutter))]
    public class GetCraftPathForCutterDetailDto : EntityDto
    {
        public string CutterNo { get; set; }
        public string Type { get; set; }
        public decimal OriginalLife { get; set; }
        public decimal CurrentLife { get; set; }
        /// <summary>
        /// 使用类型
        /// </summary>
        public EnumFmsUseType UseType { get; set; }

        /// <summary>
        /// 计数类型
        /// </summary>
        public EnumFmsCutterCountType CountType { get; set; }

        /// <summary>
        /// 刀具状态
        /// </summary>
        public EnumFmsCutterState State { get; set; }
    }
}
