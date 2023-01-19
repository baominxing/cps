namespace Wimi.BtlCore.Cutter.Dto
{
    public class CutterStatesDto : CutterStatesEntityDto
    {
        /// <summary>
        /// 计数方式名称
        /// </summary>
        public string CountingMethodName { get; set; }

        /// <summary>
        /// 刀具型号名称
        /// </summary>
        public string CutterModelName { get; set; }

        /// <summary>
        /// 刀具类型名称
        /// </summary>
        public string CutterTypeName { get; set; }

        /// <summary>
        /// 寿命状态名称
        /// </summary>
        public string LifeStatusName { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string MachineNo { get; set; }

        /// <summary>
        /// 使用状态名称
        /// </summary>
        public string UsedStatusName { get; set; }
    }
}