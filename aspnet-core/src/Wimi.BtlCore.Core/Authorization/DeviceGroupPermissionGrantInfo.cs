namespace Wimi.BtlCore.Authorization
{
    public class DeviceGroupPermissionGrantInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceGroupPermissionGrantInfo"/> class. 
        ///     Creates a new instance of <see cref="DeviceGroupPermissionGrantInfo"/>.
        /// </summary>
        /// <param name="deviceGroupId">
        /// </param>
        /// <param name="isGranted">
        /// </param>
        public DeviceGroupPermissionGrantInfo(int deviceGroupId, bool isGranted)
        {
            this.DeviceGroupId = deviceGroupId;
            this.IsGranted = isGranted;
        }

        /// <summary>
        ///     id of the permission.
        /// </summary>
        public int DeviceGroupId { get; private set; }

        /// <summary>
        ///     Is this permission granted Prohibited?
        /// </summary>
        public bool IsGranted { get; private set; }
    }
}