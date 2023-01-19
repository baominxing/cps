using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wimi.BtlCore.RDLCReport.Dto
{
    public  class PersonPerfomanceDto
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string MachineName { get; set; }
        public int MachineId { get; set; }
        public string Date { get; set; }
        public DateTime? DateTime { get; set; }
        public string State { get; set; }
        public string StateCode { get; set; }
        /// <summary>
        /// 持续时间
        /// </summary>
        public decimal Duration { get; set; }
        /// <summary>
        /// 百分比
        /// </summary>
        public string Persent { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

    }
}
