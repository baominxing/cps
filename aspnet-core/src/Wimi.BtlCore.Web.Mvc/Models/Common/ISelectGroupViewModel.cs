using System.Collections.Generic;
using Wimi.BtlCore.Web.Models.Common.Modals;

namespace Wimi.BtlCore.Web.Models.Common
{
    public interface ISelectGroupViewModel<T>
    {
        IEnumerable<SelectGroupItemViewModal<T>> Items { get; set; }
    }
}