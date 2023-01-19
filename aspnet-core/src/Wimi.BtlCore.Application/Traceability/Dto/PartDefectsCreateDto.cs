using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.Collections.Generic;
using Wimi.BtlCore.Order.PartDefects;

namespace Wimi.BtlCore.Traceability.Dto
{
    [AutoMap(typeof(PartDefect))]
    public class PartDefectsCreateDto : FullAuditedEntityDto
    {
        public string PartNo { get; set; }

        public int DefectivePartId { get; set; }

        public List<int> DefectiveReasonsId { get; set; }

        public int DefectiveMachineId { get; set; }
    }
}
