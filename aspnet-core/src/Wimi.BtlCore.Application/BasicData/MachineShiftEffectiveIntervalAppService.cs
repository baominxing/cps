using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore;
using Wimi.BtlCore.BasicData.Dto;
using Wimi.BtlCore.BasicData.Shifts;

namespace Wimi.BtlCore.BasicData
{
    public class MachineShiftEffectiveIntervalAppService:BtlCoreAppServiceBase, IMachineShiftEffectiveIntervalAppService
    {
        private readonly IRepository<MachineShiftEffectiveInterval> shiftEffectiveIntervalRepository;
        private const int ExpiryDay = 3;

        public MachineShiftEffectiveIntervalAppService(IRepository<MachineShiftEffectiveInterval> shiftEffectiveIntervalRepository)
        {
            this.shiftEffectiveIntervalRepository = shiftEffectiveIntervalRepository;
        }

        public async Task<IEnumerable<MachineShiftEffectiveIntervalDto>> ListShiftEffectiveIntervals()
        {
            var today = DateTime.Today;
            var expiryDate = today.AddDays(ExpiryDay);
            var query = await this.shiftEffectiveIntervalRepository.GetAll().Where(s =>s.EndTime>= today && s.EndTime <= expiryDate).Select(
                s => new MachineShiftEffectiveIntervalDto()
                {
                    EndTime = s.EndTime,
                    MachineId = s.MachineId,
                    MachineName = s.Machine.Name,
                    ShiftSolutionId = s.ShiftSolutionId,
                    ShiftSolutionName = s.ShiftSolution.Name,
                    StartTime = s.StartTime
                }).ToListAsync();

            return query;
        }
    }
}