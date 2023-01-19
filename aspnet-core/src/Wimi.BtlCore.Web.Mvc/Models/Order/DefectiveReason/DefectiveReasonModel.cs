using Wimi.BtlCore.Order.DefectiveReasons.Dtos;

namespace Wimi.BtlCore.Web.Models.Order
{
    public class DefectiveReasonModel : DefectiveReasonDto
    {
        public DefectiveReasonModel()
        {
            this.IsEditMode = false;
        }

        public bool IsEditMode { get; set; }
    }
}
