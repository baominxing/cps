using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.BasicData.States;
using Wimi.BtlCore.Feedback;

namespace Wimi.BtlCore.BasicData.Machines.Manager
{
    public class StateInfoManager : BtlCoreDomainServiceBase, IStateInfoManager
    {
        private readonly IRepository<StateInfo> stateInfoRepository;

        private readonly IRepository<State, long> stateRepository;

        private readonly IRepository<ReasonFeedbackRecord> reasonFeedbackRepository;

        private readonly IRepository<Machine> machineRepository;

        public StateInfoManager(IRepository<StateInfo> stateInfoRepository, IRepository<State, long> stateRepository, IRepository<ReasonFeedbackRecord> reasonFeedbackRepository, IRepository<Machine> machineRepository)
        {
            this.stateInfoRepository = stateInfoRepository;
            this.reasonFeedbackRepository = reasonFeedbackRepository;
            this.machineRepository = machineRepository;
            this.stateRepository = stateRepository;
        }
        public bool IsInUsing(StateInfo input)
        {
            var result = false;
            if (input.Type == EnumMachineStateType.Reason)
            {
                result = this.reasonFeedbackRepository.GetAll().Where(r => r.StateId.Equals(input.Id))
                    .Join(this.machineRepository.GetAll(), r => r.MachineId, m => m.Id, (r, m) => new { ReasonFeedbackRecord = r, Machine = m })
                    .Any();
            }
            else
            {
                result = this.stateRepository.GetAll().Where(s => s.Code.Equals(input.Code))
                   .Join(this.machineRepository.GetAll(), s => s.MachineId, m => m.Id, (s, m) => new { State = s, Machine = m })
                   .Any();
            }

            return result;
        }

        public async Task<IEnumerable<NameValueDto>> ListFeedbackStates()
        {
            var query = this.stateInfoRepository.GetAll().Where(s => s.Type == EnumMachineStateType.Reason)
                .Select(t => new NameValueDto { Name = t.DisplayName, Value = t.Code });
            return await query.ToListAsync();
        }

    }
}
