using System;

namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class AlarmInfoDto
    {
        public string Code { get; set; }

        public DateTime CreationTime { get; set; }

        public int CreatorUserId { get; set; }

        public int MachineId { get; set; }

        public int MachineTypeId { get; set; }

        public string Message { get; set; }

        public string Reason { get; set; }
    }
}
