using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Web.Models.Common
{
    public class FeatureTreeItemModel
    {
        public IFeatureEditViewModel EditModel { get; set; }

        public string ParentName { get; set; }

        public FeatureTreeItemModel()
        {

        }

        public FeatureTreeItemModel(IFeatureEditViewModel editModel, string parentName)
        {
            EditModel = editModel;
            ParentName = parentName;
        }
    }

}
