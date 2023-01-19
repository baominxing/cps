using Castle.Components.DictionaryAdapter;
using System.Collections.Generic;

namespace Wimi.BtlCore.Common.Dtos
{
    public class ShiftEffSummary
    {
        public ShiftEffSummary()
        {
            this.EffSummaryObjects = new EditableList<EffSummaryObject>();
        }

        public List<EffSummaryObject> EffSummaryObjects { get; set; }

        public bool IsCurrentShift { get; set; }

        public string ShiftDesc { get; set; }
    }
}
