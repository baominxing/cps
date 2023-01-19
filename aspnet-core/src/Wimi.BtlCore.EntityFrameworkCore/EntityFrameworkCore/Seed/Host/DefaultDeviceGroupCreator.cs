using System;
using System.Linq;
using Wimi.BtlCore.Authorization.Roles;
using Wimi.BtlCore.BasicData.DeviceGroups;

namespace Wimi.BtlCore.EntityFrameworkCore.Seed.Host
{
    public class DefaultDeviceGroupCreator
    {
        private readonly BtlCoreDbContext context;

        public DefaultDeviceGroupCreator(BtlCoreDbContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            if (!this.context.DeviceGroups.Any())
            {
                // 增加默认设备组
                var deviceGroup = new DeviceGroup
                {
                    Code = "00000",
                    DisplayName = "默认组",
                    IsDeleted = false,
                    ParentId = null,
                    DmpGroupId = Guid.NewGuid(),
                    CreationTime = DateTime.Now
                };
                this.context.DeviceGroups.Add(deviceGroup);
                this.context.SaveChanges();

                var machines = this.context.Machines.Where(q => q.IsDeleted == false && q.IsActive).ToArray();

                // 增加组内设备
                foreach (var machine in machines)
                    this.context.MachineDeviceGroups.Add(
                        new MachineDeviceGroup
                        {
                            MachineId = machine.Id,
                            DeviceGroupId = deviceGroup.Id,
                            CreationTime = DateTime.Now
                        });
                this.context.SaveChanges();

                // 赋权
                // get admin role
                var adminRole = this.context.Roles.FirstOrDefault(r => r.Name == StaticRoleNames.Tenants.Admin);
                this.context.DeviceGroupRolePermissions.Add(
                    new DeviceGroupRolePermissionSetting
                    {
                        RoleId = adminRole.Id,
                        DeviceGroupId = deviceGroup.Id,
                        IsGranted = true,
                        CreationTime = DateTime.Now
                    });

                this.context.SaveChanges();
            }
        }
    }
}
