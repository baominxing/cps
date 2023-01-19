using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Wimi.BtlCore.BasicData.Shifts;

namespace Wimi.BtlCore.Shift.Dtos
{
    [AutoMapFrom(typeof(ShiftSolutionItem))]
    public class ShiftInfoDto2 : AuditedEntityDto
    {
        public decimal Duration { get; set; }

        public string EndTime { get; set; }

        public string Name { get; set; }

        public int ShiftSolutionId { get; set; }

        public string StartTime { get; set; }
    }
}
