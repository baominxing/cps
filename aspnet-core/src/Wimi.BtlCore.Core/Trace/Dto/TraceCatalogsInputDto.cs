using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore.Trace.Dto
{
    public class TraceCatalogsInputDto
    {
        public TraceCatalogsInputDto()
        {
            this.UnionTables = new Tuple<List<string>, List<string>>(new List<string> { }, new List<string> { });
            this.MachineId = new List<int>();
        }

        public int Id { get; set; }

        public int? DeviceGroupId { get; set; }

        public string PartNo { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public List<int> MachineId { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public string StationCode { get; set; }

        public long NgPartCatlogId { get; set; }

        public DateTime? StartFirstTime { get; set; }

        public DateTime? StartLastTime { get; set; }

        public DateTime? EndFirstTime { get; set; }

        public DateTime? EndLastTime { get; set; }
        public string StartDate { get; set; }

        public string EndDate { get; set; }
        public Tuple<List<string>, List<string>> UnionTables { get; set; }
    }
}
