namespace Wimi.BtlCore.Shifts.Dto
{
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;
    using Wimi.BtlCore.BasicData.Shifts;

    [AutoMapFrom(typeof(ShiftSolution))]
    public class ShiftSolutionDto : AuditedEntityDto
    {
        public int MemberCount { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }
    }
}