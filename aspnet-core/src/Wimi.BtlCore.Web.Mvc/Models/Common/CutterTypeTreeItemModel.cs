using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Web.Models.Common
{
    public class CutterTypeTreeItemModel
    {
        public ICutterTypeViewModal EditModel { get; set; }

        public int? PId { get; set; }

        public CutterTypeTreeItemModel()
        {

        }

        public CutterTypeTreeItemModel(ICutterTypeViewModal editModel, int? pId)
        {
            EditModel = editModel;
            PId = pId;
        }
    }
}
