namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class GanttChartOutputDto
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 结果集合
        /// </summary>
        public Intervals[] Intervals { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否显示X轴
        /// </summary>
        public bool ShowXAxis { get; set; }
    }
}
