using System.Threading.Tasks;

using Abp.Domain.Repositories;
using Abp.Domain.Services;

namespace Wimi.BtlCore.StaffPerformance
{
    public class PerformancePersonnelManager : BtlCoreDomainServiceBase, IPerformancePersonnelManager
    {
        private readonly IRepository<PerformancePersonnelOnDevice> performancePersonnelOnDeviceRepository;

        public PerformancePersonnelManager(
            IRepository<PerformancePersonnelOnDevice> performancePersonnelOnDeviceRepository)
        {
            this.performancePersonnelOnDeviceRepository = performancePersonnelOnDeviceRepository;
        }

        public async Task<bool> DeviceIsUsed(int machineId)
        {
            var entity =
                await this.performancePersonnelOnDeviceRepository.FirstOrDefaultAsync(p => p.MachineId == machineId);
            return entity != null;
        }
    }
}