using Abp.AutoMapper;
using System;

namespace Wimi.BtlCore.Carton.CartonTraceability.Dtos
{
    [AutoMap(typeof(Cartons.Carton))]
    public class CartonTraceabilityDto
    {
        public int Id { get; set; }

        public string CartonNo { get; set; }

        public string DeviceGroupName { get; set; }

        public int DeviceGroupId { get; set; }

        public int MaxPackingCount { get; set; }

        public int RealPackingCount { get; set; }

        public int PrintLabelCount { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
