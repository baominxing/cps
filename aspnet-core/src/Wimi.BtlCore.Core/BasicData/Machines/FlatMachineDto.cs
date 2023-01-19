namespace Wimi.BtlCore.BasicData.Machines
{
    using System;

    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;

    [Serializable]
    [AutoMapFrom(typeof(Machine))]
    public class FlatMachineDto : EntityDto
    {
        /// <summary>
        ///     机器编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     机器描述
        /// </summary>
        public string Desc { get; set; }

        public int DeviceGroupId { get; set; }

        /// <summary>
        ///     图片报错Id，在表AppBinaryObjects中
        /// </summary>
        public Guid? ImageId { get; set; }

        /// <summary>
        ///     是否可用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        ///     机器名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     机器排列顺序
        /// </summary>
        public int SortSeq { get; set; }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return this.Id.GetHashCode();
        }
    }
}