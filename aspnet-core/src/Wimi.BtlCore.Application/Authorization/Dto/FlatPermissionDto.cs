namespace Wimi.BtlCore.Authorization.Dto
{
    using Abp.Authorization;
    using Abp.AutoMapper;

    [AutoMapFrom(typeof(Permission))]
    public class FlatPermissionDto
    {
        public string Description { get; set; }

        public string DisplayName { get; set; }

        public bool IsGrantedByDefault { get; set; }

        public string Name { get; set; }

        public string ParentName { get; set; }
    }
}
