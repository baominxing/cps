namespace Wimi.BtlCore.BasicData.Dto
{
    using System.Collections.Generic;

    public class MongoMachineInfoDto
    {
        public MongoMachineInfoDto()
        {
            this.ParamsItemList = new List<MachineDetailItem>();
            this.ProgramCount = "0";
            this.IsHadProgram = false;
            this.AlarmItems = new List<AlarmItem>();
        }

        /// <summary>
        /// 报警信息列表
        /// </summary>
        public IEnumerable<AlarmItem> AlarmItems { get; set; }

        /// <summary>
        /// 产量
        /// </summary>
        public decimal Count { get; set; }

        /// <summary>
        /// 是否能采集到程序
        /// </summary>
        public bool IsHadProgram { get; set; }

        /// <summary>
        /// 参数对应的值
        /// </summary>
        public List<MachineDetailItem> ParamsItemList { get; set; }

        /// <summary>
        /// 程序累计产量
        /// </summary>
        public string ProgramCount { get; set; }

        /// <summary>
        /// 程序名
        /// </summary>
        public string ProgramName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 状态持续时长=当前时间-Mongo中State的CreationTime，分钟
        /// </summary>
        public string StatusDuration { get; set; }
    }
}