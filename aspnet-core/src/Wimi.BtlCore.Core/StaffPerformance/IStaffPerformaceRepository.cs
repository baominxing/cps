namespace Wimi.BtlCore.StaffPerformance
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abp.Dependency;

    public interface IStaffPerformaceRepository : ITransientDependency
    {
        Task<IEnumerable<StaffPerformanceState>> GetStaffReasonPerformance(StaffPerformance input);

        Task<IEnumerable<StaffPerformanceState>> GetStaffStatePerformance(StaffPerformance input);

        Task<IEnumerable<dynamic>> CapacityQuery(IEnumerable<long> machineIds, DateTime startTime, DateTime endTime, List<string> unionTables);
    }
}