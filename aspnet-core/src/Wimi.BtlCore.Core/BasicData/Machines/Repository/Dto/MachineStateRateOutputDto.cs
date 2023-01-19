namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class MachineStateRateOutputDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 状态码名称
        /// </summary>
        public string DisplayName { get; set; }

        public int MachineId { get; set; }

        /// <summary>
        /// 比率
        /// </summary>
        public decimal Rate { get; set; }
    }
}
