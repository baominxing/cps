using Abp.Domain.Entities.Auditing;

namespace WIMI.BTL.ReasonFeedbackCalendar.Dtos
{
    public class SelectMachineRelatedDto: AuditedEntity
    { 
        public string MachineCode { get; set; }

        public string MachineName { get; set; }
    }
}