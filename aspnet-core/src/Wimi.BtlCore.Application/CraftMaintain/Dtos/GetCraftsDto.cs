using Abp.Application.Services.Dto;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using System.Collections.Generic;

namespace Wimi.BtlCore.CraftMaintain.Dtos
{
    [AutoMap(typeof(FlexibleCraft))]
    public class GetCraftsDto : EntityDto
    {
        public string Name { get; set; }
        public string Version { get; set; }

        [Ignore]
        public List<FlexibleCraftProcesseDto> CraftProcesses { get; set; }
        = new List<FlexibleCraftProcesseDto>();
    }

    [AutoMap(typeof(FlexibleCraft))]
    public class GetAllCraftsDto : EntityDto
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }
}
