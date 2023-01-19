namespace Wimi.BtlCore.StaffPerformances.Dto
{
    using Abp.Domain.Entities;

    public class GetDevicesRequestDto : Entity
    {
        /// <summary>
        /// 设备组Id
        /// </summary>
        public int DeviceGroupId { get; set; }

        /// <summary>
        /// 是否已上线
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        public int MachineId { get; set; }

        /// <summary>
        /// 设备图片路径
        /// </summary>
        public string MachineImageSrc { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// 上线日期
        /// </summary>
        public string OnlineDate { get; set; }

        /// <summary>
        /// 上线用户姓名
        /// </summary>
        public string PersonnelName { get; set; }

        /// <summary>
        /// 上线用户key
        /// </summary>
        public long? PersonnelUserId { get; set; }
    }
}