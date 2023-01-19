using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Dmps
{
    /// <summary>
    /// 设备网络(DMP)配置信息
    /// </summary>
    [Table("MachineNetConfigs")]
    public class MachineNetConfig : CreationAuditedEntity
    {
        [Comment("设备编号")]
        public string MachineCode { get; set; }

        [Comment("设备唯一标识(GUID)")]
        public Guid DmpMachineId { get; set; }

        [MaxLength(BtlCoreConsts.IPAdressLength)]
        [Comment("IP地址")]
        public string IpAddress { get; set; }

        [Comment("端口")]
        public int? TcpPort { get; set; }
    }
}
