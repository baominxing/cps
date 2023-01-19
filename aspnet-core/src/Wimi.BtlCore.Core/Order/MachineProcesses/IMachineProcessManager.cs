using System.Threading.Tasks;

using Abp.Domain.Services;

namespace Wimi.BtlCore.Order.MachineProcesses
{
    public interface IMachineProcessManager : IDomainService
    {
        void CheckExsist(int id);

        Task ChangeMachineProduct(MachineProcess input);
    }
}
