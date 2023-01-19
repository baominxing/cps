namespace Wimi.BtlCore.BasicData.Dto
{
    using System.Collections.Generic;
    using Abp.Domain.Entities;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.BasicData.StateInfos;

    public class MachineStatusListDto : Entity
    {
        public string Code { get; set; }

        public List<int> GroupId { get; set; }

        public string ImagePath { get; set; }

        public Machine Machine { get; set; }

        public int MachineId { get; set; }

        public MongoMachineInfoDto MongoMachineInfo { get; set; }

        public string Name { get; set; }

        public StateInfo StatusInfo { get; set; }
    }
}