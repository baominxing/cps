namespace Wimi.BtlCore.StaffPerformances.Dto
{
    using Wimi.BtlCore.StaffPerformance;

    public class GetDevicesDto
    {
        public int[] DeviceGroupIds { get; set; }

        public int[] DeviceIds { get; set; }

        /// <summary>
        /// 使用状态： 0所有，1上线，2下线，3我的
        /// </summary>
        public EnumDeviceUseState UseState { get; set; }
    }
}