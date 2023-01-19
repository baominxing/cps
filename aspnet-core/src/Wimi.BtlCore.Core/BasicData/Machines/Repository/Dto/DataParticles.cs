namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class DataParticles
    {
        /// <summary>
        /// 状态颜色 
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 结束时间 formart: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string EndDatetime { get; set; }

        /// <summary>
        /// 鼠标hover时显示的附加信息 例如 { memo: "testing" }
        /// </summary>
        public object Message { get; set; }

        /// <summary>
        /// 开始时间 formart: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string StartDatetime { get; set; }
    }
}
