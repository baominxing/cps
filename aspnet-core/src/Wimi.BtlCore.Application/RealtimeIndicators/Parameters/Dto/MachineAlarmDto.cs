namespace Wimi.BtlCore.RealtimeIndicators.Parameters.Dto
{
    /// <summary>
    /// Mongo Alarm document ¶ÔÓ¦µÄ×Ö¶Î
    /// </summary>
    public class MachineAlarmDto
    {
        public MachineAlarmDto()
        {
            this.IsAlarming = false;
        }

        public string Code { get; set; }

        public string CreationTime { get; set; }

        public bool IsAlarming { get; set; }

        public string Message { get; set; }
    }
}