using Abp.Domain.Repositories;
using Abp.UI;
using System.Linq;

namespace Wimi.BtlCore.BasicData.Machines.Manager
{
    public class MachineTypeManager : BtlCoreDomainServiceBase, IMachineTypeManager
    {
        private readonly IRepository<Machine> machineRepository;

        public MachineTypeManager(
            IRepository<Machine> machineRepository)
        {
            this.machineRepository = machineRepository;
        }

        public void CheckIsInMachine(int machineTypeId)
        {
            var query = this.machineRepository.GetAll().Where(m => m.MachineTypeId == machineTypeId);

            if (query != null && query.Count() != 0)
            {
                throw new UserFriendlyException(this.L("DevicesAreInUse{0}", string.Join(",", query.Select(q => q.Name).Distinct().ToArray())));
            }
        }
    }
}
