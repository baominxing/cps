using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Web.Models.Common.Modals
{
    public class SelectGroupItemViewModal<T>
    {
        public string Name { get; set; }


        public IEnumerable<T> ChildNode { get; set; }
    }
}
