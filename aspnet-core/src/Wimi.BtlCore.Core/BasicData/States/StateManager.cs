using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using Wimi.BtlCore.BasicData.States;

namespace Wimi.BtlCore.States
{
    public class StateManager : BtlCoreDomainServiceBase
    {
        private readonly IRepository<State, long> stateRepository;

        public StateManager(IRepository<State, long> stateRepository)
        {
            this.stateRepository = stateRepository;
        }

        public IEnumerable<State> GetLastStateRecord(string machineCode, int limitNum)
        {
            var query = this.stateRepository.GetAll()
                .Where(x => x.MachineCode == machineCode)
                .OrderByDescending(x => x.StartTime)
                .Take(limitNum);


            var result = query.ToList();
            return result;
        }

    }
}
