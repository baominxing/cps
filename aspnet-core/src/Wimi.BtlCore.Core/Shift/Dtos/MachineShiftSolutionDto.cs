namespace Wimi.BtlCore.Shift.Dtos
{
    public class MachineShiftSolutionDto
    {
        public string CreationTime { get; set; }

        public string EndTime { get; set; }

        public int Id { get; set; }

        public int MachineGroupId { get; set; }

        public string MachineGroupName { get; set; }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public int ShiftSolutionId { get; set; }

        public string ShiftSolutionName { get; set; }

        public string StartTime { get; set; }
    }
}
