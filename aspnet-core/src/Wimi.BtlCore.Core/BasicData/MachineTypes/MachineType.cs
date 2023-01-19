using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.BasicData.MachineTypes
{
    /// <summary>
    /// 设备类型表
    /// </summary>
    [Table("MachineTypes")]
    public class MachineType : CreationAuditedEntity, IDeletionAudited
    {
        
        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        /// <summary>
        /// 设备描述
        /// </summary>
        [Comment("设备类型描述")]
        public string Desc { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [Comment("设备类型名称")]
        public string Name { get; set; }
    }
}
