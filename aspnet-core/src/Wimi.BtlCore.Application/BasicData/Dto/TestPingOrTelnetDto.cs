using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.BasicData.Dto
{
    public class TestPingOrTelnetDto
    {
        public int MachineId { get; set; }

        [MaxLength(BtlCoreConsts.IPAdressLength)]
        public string IpAddress { get; set; }

        public int? TcpPort { get; set; }
    }
}
