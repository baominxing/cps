namespace Wimi.BtlCore.Web.Models.Order.DefectiveReason
{
    public class CreateDefectivePartModal
    {
        public CreateDefectivePartModal(long? parentId)
        {
            this.ParentId = parentId;
        }
        public long? ParentId { get; set; }
    }
}
