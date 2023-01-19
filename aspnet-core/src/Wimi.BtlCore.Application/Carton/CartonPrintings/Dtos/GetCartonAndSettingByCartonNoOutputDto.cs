namespace Wimi.BtlCore.Carton.CartonPrintings.Dtos
{
    public class GetCartonAndSettingByCartonNoOutputDto
    {
        public GetCartonAndSettingByCartonNoOutputDto()
        {
            CartonSetting = new GetCartonSettingOutputDto();

            CartonInfo = new PackingOutputDto();
        }

        public GetCartonSettingOutputDto CartonSetting { get; set; }

        public PackingOutputDto CartonInfo { get; set; }
    }
}
