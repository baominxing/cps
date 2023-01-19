using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.BasicData.DeviceGroups
{
    /// <summary>
    ///     Used to store setting for a machine unit permission for a role.
    /// </summary>
    [Table("DeviceGroupPermissions")]
    public class DeviceGroupPermissionSetting : CreationAuditedEntity<long>, IMayHaveTenant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceGroupPermissionSetting"/> class. 
        ///     Creates a new <see cref="PermissionSetting"/> entity.
        /// </summary>
        protected DeviceGroupPermissionSetting()
        {
            this.IsGranted = true;
        }

        [ForeignKey("DeviceGroupId")]
        [Comment("设备组")]
        public virtual DeviceGroup DeviceGroup { get; set; }

        /// <summary>
        ///     设备组Id.
        /// </summary>
        [Comment("设备组Id")]
        public virtual int DeviceGroupId { get; set; }

        /// <summary>
        ///     Is this role granted for this permission.
        ///     Default value: true.
        /// </summary>
        [Comment("是否赋予权限")]
        public virtual bool IsGranted { get; set; }

        [Comment("租户Id")]
        public virtual int? TenantId { get; set; }
    }
}
