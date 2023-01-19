namespace Wimi.BtlCore.Shifts.Dto
{
    using System;

    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;
    using Wimi.BtlCore.BasicData.Shifts;

    [AutoMapFrom(typeof(ShiftSolutionItem))]
    public class ShiftInfoDto : AuditedEntityDto
    {
        public decimal Duration { get; set; }

        public DateTime EndTime { get; set; }

        public string Name { get; set; }

        public int ShiftSolutionId { get; set; }

        public DateTime StartTime { get; set; }

        public bool IsNextDay { get; set; }
    }
}