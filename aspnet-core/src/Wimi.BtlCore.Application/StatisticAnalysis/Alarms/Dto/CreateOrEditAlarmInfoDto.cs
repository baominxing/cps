namespace Wimi.BtlCore.StatisticAnalysis.Alarms.Dto
{
    using System.ComponentModel.DataAnnotations;

    using Abp.AutoMapper;
    using Abp.Domain.Entities.Auditing;
    using Wimi.BtlCore.BasicData.Alarms;

    [AutoMap(typeof(AlarmInfo))]
    public class CreateOrEditAlarmInfoDto : CreationAuditedEntity<long?>
    {
        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Code { get; set; }

        public int MachineId { get; set; }

        [MaxLength(BtlCoreConsts.MaxDescLength * 4)]
        public string Message { get; set; }

        public string Name { get; set; }

        [MaxLength(BtlCoreConsts.MaxDescLength * 4)]
        public string Reason { get; set; }

        public string MachineName { get; set; }

        public string MachineCode { get; set; }

        public int SortSeq { get; set; }
    }
}