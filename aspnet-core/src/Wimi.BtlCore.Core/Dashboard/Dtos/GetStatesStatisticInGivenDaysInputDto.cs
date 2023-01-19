namespace Wimi.BtlCore.Dashboard.Dtos
{
    public class GetStatesStatisticInGivenDaysInputDto
    {
        public int DateEnd { get; set; }

        public int DateFrom { get; set; }

        public int GroupId { get; set; }

        public int MachineId { get; set; }
    }
}
