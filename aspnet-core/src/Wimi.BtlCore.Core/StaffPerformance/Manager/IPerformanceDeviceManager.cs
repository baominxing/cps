namespace Wimi.BtlCore.StaffPerformance.Manager
{
    using System.Threading.Tasks;

    using Abp.Domain.Services;

    public interface IPerformanceDeviceManager : IDomainService
    {
        Task Offline(int machineId, long userId);

        Task Online(int machineId, long userId, int shiftId);
    }
}