namespace Wimi.BtlCore.RealtimeIndicators.Parameters.Dto
{
    using System.Collections.Generic;
    using Wimi.BtlCore.CommonEnums;

    public class MachineStateInputDto
    {
        public MachineStateInputDto()
        {
            this.MachineIds = new List<int>();
            this.DeviceIdGroupIds = new List<int>();
        }

        public int PageNo { get; set; }

        public int PageSize { get; set; }

        public List<int> MachineIds { get; set; }

        public List<int> DeviceIdGroupIds { get; set; }

        public string[] StateCodes { get; set; }

        public EnumQueryMethod Type { get; set; }
    }
}