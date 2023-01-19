using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Web.Models.Common
{
    public class GrantedDeviceGroupTreeItemModel
    {
        public IDeviceGroupsWithPermissionsViewModal EditModel { get; set; }

        public int? ParentId { get; set; }

        public GrantedDeviceGroupTreeItemModel(IDeviceGroupsWithPermissionsViewModal editModel, int? parentId)
        {
            this.EditModel = editModel;
            this.ParentId = parentId;
        }
    }
}
