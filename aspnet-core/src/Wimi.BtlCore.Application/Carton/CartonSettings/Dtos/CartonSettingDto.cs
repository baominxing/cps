using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using Wimi.BtlCore.Cartons;

namespace Wimi.BtlCore.Carton.CartonSettings.Dtos
{
    [AutoMap(typeof(CartonSetting))]
    public class CartonSettingDto : FullAuditedEntity
    {
        public int DeviceGroupId { get; set; }

        public int MaxPackingCount { get; set; }

        public bool IsPrint { get; set; }

        public string PrinterName { get; set; }

        public bool AutoCartonNo { get; set; }

        public int? CartonRuleId { get; set; }

        public bool IsGoodOnly { get; set; }

        public bool IsAutoPrint { get; set; }

        public bool ForbidHopSequence { get; set; }

        public bool ForbidRepeatPacking { get; set; }

        public bool IsUnpackingRedo { get; set; }

        public bool IsUnpackingAfterPrint { get; set; }

        public bool IsFinalTest { get; set; }

        public bool HasToFlow { get; set; }

        public string FlowIds { get; set; }
    }
}
