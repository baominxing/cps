using Abp.Domain.Services;

namespace Wimi.BtlCore.BasicData.Machines.Manager
{
    public interface IMachineTypeManager : IDomainService
    {
        void CheckIsInMachine(int machineTypeId);
    }
}
