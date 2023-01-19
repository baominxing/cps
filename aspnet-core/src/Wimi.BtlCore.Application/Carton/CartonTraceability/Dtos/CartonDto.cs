using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using Wimi.BtlCore.BasicData.DeviceGroups;

namespace Wimi.BtlCore.Carton.CartonTraceability.Dtos
{
    [AutoMap(typeof(Cartons.Carton))]
    public class CartonDto : FullAuditedEntity
    {
        public string CartonNo { get; set; }

        public int DeviceGroupId { get; set; }

        public virtual DeviceGroup DeviceGroup { get; set; }

        public int MaxPackingCount { get; set; }

        public int RealPackingCount { get; set; }

        public int PrintLabelCount { get; set; }
    }
}
