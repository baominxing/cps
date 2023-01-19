namespace Wimi.BtlCore.StatisticAnalysis.Yield.Dto
{
    using System;
    using System.Collections.Generic;

    using Castle.Components.DictionaryAdapter;

    public class MachineDetailYieldInfoInputDto
    {
        public MachineDetailYieldInfoInputDto()
        {
            this.MachineIdList = new EditableList<int>();
        }

        public DateTime? CurrentQueryDate { get; set; }

        public int? MachineId { get; set; }

        public List<int> MachineIdList { get; set; }

        public string SummaryDate { get; set; }

        public bool IsHistoryDay { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? StartTime { get; set; }
    }
}