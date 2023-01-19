using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.CraftMaintain.Dtos
{
    [AutoMap(typeof(FlexibleCraft))]
    public class CreatCraftInput : NullableIdDto
    {
        public int ProductId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }
        public string Version { get; set; }

        public List<int> CraftProcesseIds { get; set; }

        public List<CraftProcedureCutterDto> CraftProcedureCutters { get; set; }
    }

    [AutoMap(typeof(FlexibleCraftProcedureCutterMap))]
    public class CraftProcedureCutterDto
    {
        public int CraftProcesseId { get; set; }
        public string ProcedureNumber { get; set; }
        public int CutterId { get; set; }
    }
}
