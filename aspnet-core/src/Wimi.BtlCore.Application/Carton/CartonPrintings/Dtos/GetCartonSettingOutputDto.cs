using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Wimi.BtlCore.Cartons;

namespace Wimi.BtlCore.Carton.CartonPrintings.Dtos
{
    [AutoMap(typeof(CartonSetting))]
    public class GetCartonSettingOutputDto : FullAuditedEntityDto
    {
        public int DeviceGroupId { get; set; }

        public int MaxPackingCount { get; set; }

        public bool IsPrint { get; set; }

        public string PrinterName { get; set; }

        public bool AutoCartonNo { get; set; }

        public int CartonRuleId { get; set; }

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
