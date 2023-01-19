using Abp.Domain.Repositories;
using System.Threading.Tasks;
using Wimi.BtlCore.Extensions;

namespace Wimi.BtlCore.Order.Processes
{
    public class ProcessManager : BtlCoreDomainServiceBase, IProcessManager
    {
        private readonly IRepository<Process> processRepository;

        public ProcessManager(IRepository<Process> processRepository)
        {
            this.processRepository = processRepository;
        }

        public async Task<bool> CodeIsExist(string code)
        {
            return await this.processRepository.IsExistAsync(c => c.Code == code);
        }

        public async Task<bool> NameIsExist(string name)
        {
            return await this.processRepository.IsExistAsync(c => c.Name == name);
        }
    }
}
