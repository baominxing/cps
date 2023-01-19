using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.ShiftDayTimeRange;

namespace Wimi.BtlCore.BasicData.Shifts
{
    public class ShiftSolutionManager : BtlCoreDomainServiceBase, IShiftSolutionManager
    {
        private readonly IRepository<ShiftSolutionItem> shiftSolutionItemRepository;
        private readonly IRepository<ShiftSolution> solutionRepository;
        private readonly IShiftDayTimeRangeRepository shiftDayTimeRangeRepository;
        private readonly IRepository<MachinesShiftDetail> machinesShiftDetailsRepository;
        private readonly IRepository<MachineShiftEffectiveInterval> machineShiftEffectiveIntervalRepository;
        private readonly ILocalizationManager localizationManager;

        public ShiftSolutionManager(
            IRepository<ShiftSolutionItem> shiftSolutionItemRepository,
            IRepository<ShiftSolution> solutionRepository,
            IShiftDayTimeRangeRepository shiftDayTimeRangeRepository,
            IRepository<MachinesShiftDetail> machinesShiftDetailsRepository,
            IRepository<MachineShiftEffectiveInterval> machineShiftEffectiveIntervalRepository,
            ILocalizationManager localizationManager)
        {
            this.shiftSolutionItemRepository = shiftSolutionItemRepository;
            this.solutionRepository = solutionRepository;
            this.shiftDayTimeRangeRepository = shiftDayTimeRangeRepository;
            this.machinesShiftDetailsRepository = machinesShiftDetailsRepository;
            this.machineShiftEffectiveIntervalRepository = machineShiftEffectiveIntervalRepository;
            this.localizationManager = localizationManager;
        }

        public async Task<IEnumerable<NameValueDto<IEnumerable<ShiftSolutionItem>>>> ListShiftSolution()
        {
            var query = this.solutionRepository.GetAll().AsEnumerable().Join(this.shiftSolutionItemRepository.GetAll().AsEnumerable(), s => s.Id,
                ss => ss.ShiftSolutionId, (s, ss) => new
                {
                    Solution = s,
                    Item = ss
                }).GroupBy(g => new { g.Solution.Id, g.Solution.Name }).Select(n =>
                  new NameValueDto<IEnumerable<ShiftSolutionItem>>()
                  {
                      Name = n.Key.Name,
                      Value = n.Select(m => m.Item)
                  }).ToList();

            await Task.FromResult(0);
            return query;
        }

        public void CheckIsInUsing(int shiftSolutionId)
        {
            var query = shiftDayTimeRangeRepository.ListMachineShiftEffectiveIntervalTimeRange(shiftSolutionId);

            if (query.Where(q => q.StartTime < DateTime.Now).Any())
            {
                throw new UserFriendlyException(localizationManager.GetString(BtlCoreConsts.LocalizationSourceName, "ShiftSolutionIsInUse"));
            }
        }

        public async Task DeleteById(int shiftSolutionId)
        {
            await this.machineShiftEffectiveIntervalRepository.DeleteAsync(msei => msei.ShiftSolutionId == shiftSolutionId);

            await this.machinesShiftDetailsRepository.DeleteAsync(msd => msd.ShiftSolutionId == shiftSolutionId);

            await this.solutionRepository.DeleteAsync(s => s.Id == shiftSolutionId);

            await this.shiftSolutionItemRepository.DeleteAsync(ssi => ssi.ShiftSolutionId == shiftSolutionId);
        }
    }
}
