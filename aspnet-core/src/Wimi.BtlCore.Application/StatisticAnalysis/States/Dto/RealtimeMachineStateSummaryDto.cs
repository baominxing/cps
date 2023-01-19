namespace Wimi.BtlCore.StatisticAnalysis.States.Dto
{
    using System.Collections.Generic;

    public class RealtimeMachineStateSummaryDto
    {
        public RealtimeMachineStateSummaryDto()
        {
            this.StateCollection = new List<StateItem>();
            SyncDataState = "OK";
        }

        public bool IsError { get; set; }

        public string Message { get; set; }

        public List<StateItem> StateCollection { get; set; }
        public string SyncDataState { get; set; }
    }
}