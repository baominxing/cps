namespace Wimi.BtlCore.RealtimeIndicators.Parameters.Dto
{
    using System.Collections.Generic;

    /// <summary>
    /// Mongo中报警相关的字段
    /// </summary>
    public class MongoAlarmInfo
    {
        public MongoAlarmInfo()
        {
            this.AlarmItems = new List<MachineAlarmDto>();
        }

        /// <summary>
        /// 报警信息
        /// </summary>
        public List<MachineAlarmDto> AlarmItems { get; set; }

        /// <summary>
        /// 设备当前状态
        /// </summary>
        public string StateCode { get; set; }
    }
}