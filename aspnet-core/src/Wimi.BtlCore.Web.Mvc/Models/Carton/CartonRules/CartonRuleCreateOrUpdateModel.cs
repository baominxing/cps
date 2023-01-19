using Wimi.BtlCore.Carton.CartonRules.Dtos;

namespace Wimi.BtlCore.Web.Models.Carton.CartonRules
{
    public class CartonRuleCreateOrUpdateModel : CartonRuleInputDto
    {
        public bool IsEditMode { get; set; }
    }
}
