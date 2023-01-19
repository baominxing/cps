using System;

using Abp.AutoMapper;
using Wimi.BtlCore.Order.DefectiveReasons;
using Wimi.BtlCore.Order.MachineDefectiveRecords;

namespace Wimi.BtlCore.Order.MachineReport.Dtos
{
    [AutoMap(typeof(MachineDefectiveRecord))]
    public class MachineShiftDefectiveAnalysisDto
    {
        public int MachineId { get; set; }

        public int DefectiveReasonsId { get; set; }

        public DefectiveReason DefectiveReason { get; set; }

        public int Count { get; set; }

        public DateTime ShiftDay { get; set; }
    }
}
