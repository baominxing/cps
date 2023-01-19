namespace Wimi.BtlCore.BasicData.DeviceGroups
{
    using System;

    using Abp.AutoMapper;

    [Serializable]
    [AutoMapFrom(typeof(DeviceGroup))]
    public class FlatDeviceGroupDto
    {
        public string Code { get; set; }

        public string DisplayName { get; set; }

        public int Id { get; set; }

        public int MemberCount { get; set; }

        public int? ParentId { get; set; }

        public int? TenantId { get; set; }

        public string DisplayNameWithGroup { get; set; }

        public int Seq { get; set; }
    }
}