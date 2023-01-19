namespace Wimi.BtlCore.Web.Models.BasicData.DeviceGroups
{
    public class CreateDeviceGroupModalViewModel
    {
        public CreateDeviceGroupModalViewModel(long? parentId)
        {
            this.ParentId = parentId;
        }

        public long? ParentId { get; set; }
    }
}
