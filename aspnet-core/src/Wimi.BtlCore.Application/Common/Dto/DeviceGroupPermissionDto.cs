namespace Wimi.BtlCore.Common.Dto
{
    public class DeviceGroupPermissionDto
    {
        public string DisplayName { get; set; }

        public int? Id { get; set; }

        public bool IsGranted { get; set; }

        public int? ParentId { get; set; }

        public int? RoleId { get; set; }

        public int? TenantId { get; set; }
    }
}