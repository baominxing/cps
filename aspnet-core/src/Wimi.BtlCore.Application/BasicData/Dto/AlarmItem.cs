namespace Wimi.BtlCore.BasicData.Dto
{
    public class AlarmItem
    {
        public int DateKey { get; set; }

        public int MachineId { get; set; }

        public int MachinesShiftDetailId { get; set; }

        /// <summary>
        /// 报警信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 报警编号
        /// </summary>
        public string No { get; set; }

        public int OrderId { get; set; }

        public string PartNo { get; set; }

        public int ProcessId { get; set; }

        public string ProgramName { get; set; }

        public int UserId { get; set; }

        public int UserShiftDetailId { get; set; }
    }
}