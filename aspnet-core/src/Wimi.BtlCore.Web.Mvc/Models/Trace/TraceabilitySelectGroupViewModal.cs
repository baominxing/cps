using System.Collections.Generic;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Web.Models.Common;
using Wimi.BtlCore.Web.Models.Common.Modals;

namespace Wimi.BtlCore.Web.Models.Trace
{
    public class TraceabilitySelectGroupViewModal : ISelectGroupViewModel<ShiftSolutionItem>
    {
        public IEnumerable<SelectGroupItemViewModal<ShiftSolutionItem>> Items { get; set; }
    }
}