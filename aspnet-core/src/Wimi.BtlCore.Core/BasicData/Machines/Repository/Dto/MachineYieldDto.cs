using System.Collections.Generic;

namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class MachineYieldDto
    {
        public List<TableData> TableDataList { get; set; } = new List<TableData>();

        public List<ChartData> ChartDataList { get; set; } = new List<ChartData>();
    }

    public class TableData
    {
        public int MachineId { get; set; }

        public string MachineName { get; set; }


        public string SummaryDate { get; set; }

        public int Yield { get; set; }
    }

    public class ChartData
    {
        public string SummaryDate { get; set; }

        public string MachineName { get; set; }

        public List<int> Yields { get; set; } = new List<int>();
    }
}
