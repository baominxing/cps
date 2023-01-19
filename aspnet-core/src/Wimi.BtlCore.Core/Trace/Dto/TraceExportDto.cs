using System;

namespace Wimi.BtlCore.Trace.Dto
{
    public class TraceExportDto
    {
        public long Id { get; set; }

        public string PartNo { get; set; }

        public DateTime OnlineTime { get; set; }

        public DateTime? OfflineTime { get; set; }

        public int DeviceGroupId { get; set; }

        public bool? Qualified { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public string Station { get; set; }

        public FlowState State { set; get; }

        public FlowTag Tag { get; set; }

        public long UserId { set; get; }

        public string UserName { get; set; }

        public bool? IsReworkPart { get; set; }

        public string QualifiedString
        {
            get
            {
                if (Qualified == true)
                {
                    return "合格";
                }
                else if (Qualified == false)
                {
                    return "不合格";
                }
                else
                {
                    return "未知";
                }
            }
        }

        //-----

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public string FlowDisplayName { get; set; }

        public DateTime EntryTime { get; set; }

        public DateTime? LeftTime { get; set; }

        public double? FlowBeat
        {
            get
            {
                if (LeftTime == null)
                {
                    return null;
                }
                return Math.Round((Convert.ToDateTime(LeftTime) - EntryTime).TotalSeconds, 2);
            }
        }

    }
}
