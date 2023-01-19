namespace Wimi.BtlCore.StatisticAnalysis.OEE.Dto
{
    public class MachineOEEDto
    {
        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public decimal Value { get; set; }

        public string ShiftDay { get; set; }
    }
}