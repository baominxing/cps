using Abp.Domain.Repositories;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Shifts.Manager.Dto;

namespace Wimi.BtlCore.BasicData.Shifts.Manager
{
    public class MachinesShiftDetailsManager : BtlCoreDomainServiceBase
    {
        private readonly IRepository<MachinesShiftDetail> machinesShiftDetailsRepository;

        public MachinesShiftDetailsManager(
            IRepository<MachinesShiftDetail> machinesShiftDetailsRepository)
        {
            this.machinesShiftDetailsRepository = machinesShiftDetailsRepository;
        }

        public async Task DeleteShiftDetailsByMachine(MachineShiftDeleteDto input)
        {
           await this.machinesShiftDetailsRepository.DeleteAsync(m=>m.MachineId == input.MachineId && m.ShiftDay>input.ShiftDay);
        }
    }
}
