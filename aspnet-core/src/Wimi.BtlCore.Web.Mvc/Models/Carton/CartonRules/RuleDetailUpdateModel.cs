using Wimi.BtlCore.Carton.CartonRules.Dtos;

namespace Wimi.BtlCore.Web.Models.Carton.CartonRules
{
    public class RuleDetailUpdateModel
    {
        public int RuleId { get; set; }

        public RuleDetailInputItem RuleDetailItem { get; set; }

        public InfosValueDto ShiftInfos { get; set; }

        public InfosValueDto DeviceGroupInfos { get; set; }
    }
}
