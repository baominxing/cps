namespace Wimi.BtlCore.Carton.CartonPrintings.Dtos
{
    public class PackingInputDto
    {
        public string PartNo { get; set; }

        public int CartonSettingId { get; set; }

        public string CartonNo { get; set; }

        public bool IsSwitchNo { get; set; }
    }
}
