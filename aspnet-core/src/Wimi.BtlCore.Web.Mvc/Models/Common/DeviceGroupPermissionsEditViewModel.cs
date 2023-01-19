using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Web.Models.Common
{
    public class DeviceGroupPermissionsEditViewModel
    {
        public IDeviceGroupPermissionsEditViewModel EditModel { get; set; }

        public int? ParentId { get; set; }

        public DeviceGroupPermissionsEditViewModel()
        {

        }

        public DeviceGroupPermissionsEditViewModel(IDeviceGroupPermissionsEditViewModel editModel, int? parentId)
        {
            EditModel = editModel;
            ParentId = parentId;
        }
    }
}
