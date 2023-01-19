namespace Wimi.BtlCore.StaffPerformances
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Abp.Specifications;
    using Wimi.BtlCore.BasicData.Capacities;

    public class CapacityMachineSpec : Specification<Capacity>
    {
        public CapacityMachineSpec(IEnumerable<long> machineIds)
        {
            this.MachineIds = machineIds;
        }

        public IEnumerable<long> MachineIds { get; private set; }

        public override Expression<Func<Capacity, bool>> ToExpression()
        {
            if (!this.MachineIds.Any())
            {
                return c => true;
            }

            return c => this.MachineIds.Contains(c.MachineId);
        }
    }
}