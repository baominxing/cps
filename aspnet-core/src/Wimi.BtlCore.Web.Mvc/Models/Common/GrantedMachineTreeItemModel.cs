using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Web.Models.Common
{
    public class GrantedMachineTreeItemModel
    {
        public IDeviceGroupAndMachineWithPermissionsViewModal EditModel { get; set; }

        public int? ParentId { get; set; }

        public GrantedMachineTreeItemModel(IDeviceGroupAndMachineWithPermissionsViewModal editModel, int? parentId)
        {
            EditModel = editModel;
            ParentId = parentId;
        }
    }
}
