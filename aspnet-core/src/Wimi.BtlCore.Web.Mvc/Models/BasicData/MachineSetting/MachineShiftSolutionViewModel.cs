using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Web.Models.BasicData.MachineSetting
{
    public class MachineShiftSolutionViewModel
    {
        public DateTime EndTime { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public int ShiftSolutionId { get; set; }

        public DateTime StartTime { get; set; }

        public bool IsInUse { get; set; }
    }
}
