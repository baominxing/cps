namespace Wimi.BtlCore.DeviceGroups.Dto
{
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;
    using Wimi.BtlCore.BasicData.DeviceGroups;

    [AutoMapFrom(typeof(DeviceGroup))]
    public class DeviceGroupDto : AuditedEntityDto
    {
        public string Code { get; set; }

        public string DisplayName { get; set; }

        public int MemberCount { get; set; }

        public int? ParentId { get; set; }

        public int Seq { get; set; }
    }
}