using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Web.Controllers.RDLCReport
{
    public class ReturnReportModel
    {
        public string FrxName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int ShiftSolutionId { get; set; }
    }
}
