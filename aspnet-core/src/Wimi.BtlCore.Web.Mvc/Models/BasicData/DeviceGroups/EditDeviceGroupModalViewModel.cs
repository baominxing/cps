using Abp.AutoMapper;
using Wimi.BtlCore.BasicData.DeviceGroups;

namespace Wimi.BtlCore.Web.Models.BasicData.DeviceGroups
{
    [AutoMapFrom(typeof(DeviceGroup))]
    public class EditDeviceGroupModalViewModel
    {
        public string DisplayName { get; set; }

        public long? Id { get; set; }

        public int Seq { get; set; }
    }
}
