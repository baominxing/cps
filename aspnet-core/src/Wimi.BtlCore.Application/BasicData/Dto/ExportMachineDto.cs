namespace Wimi.BtlCore.BasicData.Dto
{
    public class ExportMachineDto
    {
        public string MachineId { get; set; }

        public string MachineName { get; set; }

        public string ParentGroupId { get; set; }

        public string GroupId { get; set; }

        public string GroupName { get; set; }

        public string GroupCode { get; set; }

        public string DriverName { get; set; } = "DMP.DeviceDriver.Virtual.VirtualDriver";

        public string TypeName { get; set; } = "WIMI VD V1.0";

        public string Enable { get; set; } = "True";
    }
}
