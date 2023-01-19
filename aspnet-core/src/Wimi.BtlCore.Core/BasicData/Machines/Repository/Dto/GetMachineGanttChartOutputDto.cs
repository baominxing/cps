using System.Collections.Generic;
using Nancy.Json;

namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class GetMachineGanttChartOutputDto
    {
        public GanttChartOutputDto ChartDataList { get; set; }

        public int MachineId { get; set; }

        [ScriptIgnore]
        public List<GetOriginalState> MachineStatesList { get; set; }
    }
}
