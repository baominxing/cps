namespace Wimi.BtlCore.Common.Dtos
{
    public class ShiftUtilizationRate
    {
        public int MachineGroupId { get; set; }

        public string MachineGroupName { get; set; }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public decimal Rate { get; set; }

        public int ShiftId { get; set; }

        public string ShiftName { get; set; }

        public string WorkShopCode { get; set; }
    }
}
