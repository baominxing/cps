using Abp.AutoMapper;

namespace Wimi.BtlCore.Carton.CartonPrintings.Dtos
{
    [AutoMap(typeof(Cartons.Carton))]
    public class PackingOutputDto
    {
        public string CartonNo { get; set; }

        public int DeviceGroupId { get; set; }

        public int MaxPackingCount { get; set; }

        public int RealPackingCount { get; set; }

        public int PrintLabelCount { get; set; }
    }
}
