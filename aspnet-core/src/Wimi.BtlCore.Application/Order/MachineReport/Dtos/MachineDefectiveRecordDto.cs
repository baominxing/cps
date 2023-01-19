using System;
using System.Collections.Generic;
using Abp.AutoMapper;
using Wimi.BtlCore.Order.MachineDefectiveRecords;

namespace Wimi.BtlCore.Order.MachineReport.Dtos
{
    [AutoMap(typeof(MachineDefectiveRecord))]
    public class MachineDefectiveRecordDto
    {
        public int? Id { get; set; }

        public int? Count { get; set; }

        public int? DefectiveReasonsId { get; set; }

        public string ReasonName { get; set; }

        public int? MachineId { get; set; }

        public int ProductId { get; set; }

        public int? ShiftSolutionItemId { get; set; }

        public DateTime CreationTime { get; set; }
    }

    public class MachinePartsDefectiveRecordDto
    {
        public string DefectivePartName { get; set; }

        public List<MachineDefectiveRecordDto> DefectiveRecordList { get; set; }
    }
}
