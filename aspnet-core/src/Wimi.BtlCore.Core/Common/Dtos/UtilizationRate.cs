using Castle.Components.DictionaryAdapter;
using System.Collections.Generic;

namespace Wimi.BtlCore.Common.Dtos
{
    public class UtilizationRate
    {
        public UtilizationRate()
        {
            this.UtilizationRateList = new EditableList<ShiftUtilizationRate>();
        }

        public string MachineGroupName { get; set; }

        public List<ShiftUtilizationRate> UtilizationRateList { get; set; }
    }
}
