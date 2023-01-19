namespace Wimi.BtlCore.Web.Models.App
{
    public class CreateOrganizationUnitModalViewModel
    {
        public CreateOrganizationUnitModalViewModel(long? parentId)
        {
            this.ParentId = parentId;
        }

        public long? ParentId { get; set; }
    }
}
