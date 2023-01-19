using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.Collections.Generic;

namespace Wimi.BtlCore.CraftMaintain.Dtos
{
    [AutoMap(typeof(FlexibleCraft))]
    public class StepOneDto : EntityDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public List<FlexibleCraftProcesseDto> Processes { get; set; }
        = new List<FlexibleCraftProcesseDto>();
    }
     
    public class FlexibleCraftProcesseDto : EntityDto
    {
        public string Name { get; set; }
        public int Sequence { get; set; }
        public string TongName { get; set; }
        public string Programes { get; set; }
    }
}
