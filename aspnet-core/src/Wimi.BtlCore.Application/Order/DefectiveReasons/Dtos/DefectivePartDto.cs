using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Wimi.BtlCore.Order.DefectiveParts;

namespace Wimi.BtlCore.Order.DefectiveReasons.Dtos
{
    [AutoMapFrom(typeof(DefectivePart))]
    public class DefectivePartDto : AuditedEntityDto
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int MemberCount { get; set; }

        public int? ParentId { get; set; }

    }
}
