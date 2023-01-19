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
        ///     �������
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     ��������
        /// </summary>
        public string Desc { get; set; }

        public int DeviceGroupId { get; set; }

        /// <summary>
        ///     ͼƬ����Id���ڱ�AppBinaryObjects��
        /// </summary>
        public Guid? ImageId { get; set; }

        /// <summary>
        ///     �Ƿ����
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        ///     ��������
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     ��������˳��
        /// </summary>
        public int SortSeq { get; set; }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return this.Id.GetHashCode();
        }
    }
}