namespace Wimi.BtlCore.DeviceGroups.Dto
{
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;
    using System;
    using Wimi.BtlCore.BasicData.Machines;

    [AutoMapFrom(typeof(Machine))]
    public class DeviceGroupMachineListDto : EntityDto
    {
        public DateTime AddedTime { get; set; }

        public string Code { get; set; }

        public string Desc { get; set; }

        public Guid? ImageId { get; set; }

        public string Name { get; set; }

        public int SortSeq { get; set; }
    }
}