namespace Wimi.BtlCore.StatisticAnalysis.Alarms.Dto
{
    using System;

    using Abp.Application.Services.Dto;

    public class MachineAlarmsDto : EntityDto<long>
    {
        public string AlarmCode { get; set; }

        public string AlarmMessage { get; set; }

        public DateTime AlarmTime { get; set; }

        public decimal? DurationSeconds { get; set; }

        public string MachineCode { get; set; }

        public long MachineId { get; set; }

        public string MachineName { get; set; }

        public string Memo { get; set; }
    }
}