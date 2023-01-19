using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wimi.BtlCore.RDLCReport.Dto
{
    public class MachineUtilizationRateDto
    {
        //展示
        public string MachineGroup { get; set; }
        public string Machine { get; set; }
        public string Date { get; set; }
        public string UtilizationRate { get; set; }
        //数据处理
        public bool IsPlan { get; set; }
        public decimal Duration { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }

    
    }
}
