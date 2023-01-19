namespace Wimi.BtlCore.Order.MachineProcesses.Dtos
{
    using System;

    using Abp.AutoMapper;

    [AutoMap(typeof(MachineProcess))]
    public class MachineProcessDto
    {

        /// <summary>
        /// 设备换产ID
        /// </summary>
        public int Id { get; set; }

        public string MachineCode { get; set; }

        public string MachineName { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public string ProcessCode { get; set; }

        public string ProcessName { get; set; }

        public DateTime CreationTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime? EndTime { get; set; }

        public string ChangeProductUserName { get; set; }

        public int MachineId { get; set; }

        public int ProductId { get; set; }

        public int ProcessId { get; set; }
    }
}
