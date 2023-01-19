namespace Wimi.BtlCore.StaffPerformance
{
    using System.Threading.Tasks;

    using Abp.Domain.Services;

    public interface IPerformancePersonnelManager : IDomainService
    {
        Task<bool> DeviceIsUsed(int machineId);
    }
}