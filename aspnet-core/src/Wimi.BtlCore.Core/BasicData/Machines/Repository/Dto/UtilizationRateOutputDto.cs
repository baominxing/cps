namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class UtilizationRateOutputDto
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public int MachineId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// 设备利用率
        /// </summary>
        public string UtilizationRate { get; set; }
    }
}
