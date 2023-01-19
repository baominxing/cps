using Wimi.BtlCore.Order.StandardTimes.Dtos;

namespace Wimi.BtlCore.Web.Models.Order.StandardTime
{
    public class StandardTimeModel : StandardTimeDto
    {
        public StandardTimeModel()
        {
            this.IsEditMode = false;
        }

        public bool IsEditMode { get; set; }
    }
}
