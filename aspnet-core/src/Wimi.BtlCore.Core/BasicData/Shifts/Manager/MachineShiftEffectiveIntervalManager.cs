using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Shifts.Manager.Dto;
using Wimi.BtlCore.Timing.Utils;

namespace Wimi.BtlCore.BasicData.Shifts.Manager
{
    public class MachineShiftEffectiveIntervalManager : BtlCoreDomainServiceBase
    {
        private readonly IRepository<MachineShiftEffectiveInterval> shiftEffectiveIntervalRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<MachinesShiftDetail> machinesShiftDetailsRepository;

        public MachineShiftEffectiveIntervalManager(IRepository<MachineShiftEffectiveInterval> shiftEffectiveIntervalRepository,
            IRepository<Machine> machineRepository,
            IRepository<MachinesShiftDetail> machinesShiftDetailsRepository)
        {
            this.shiftEffectiveIntervalRepository = shiftEffectiveIntervalRepository;
            this.machineRepository = machineRepository;
            this.machinesShiftDetailsRepository = machinesShiftDetailsRepository;
        }

        public async Task<string> ListMachineShiftNotScheduling()
        {
            var today = DateTime.Now.ConvertTodayToCstTime();
            var machineList = await machineRepository.GetAll().Where(m => m.IsActive).ToListAsync();
            var returnValue = new List<string>();

            foreach(var machine in machineList)
            {
                var isExist = machinesShiftDetailsRepository.GetAll().Any(m => m.ShiftDay == today&&m.MachineId== machine.Id);
                if (!isExist)
                {
                    returnValue.Add(machine.Name);
                }
            }
 
            return returnValue.JoinAsString(",");

        }

        public async Task<bool> IsMachineShiftExpiry()
        {
            //var today = DateTime.Today;
            //var expiryDate = today.AddDays(3);

            //var query = from m in this.machineRepository.GetAll()
            //    join s in this.shiftEffectiveIntervalRepository.GetAll() on m.Id equals s.MachineId
            //    select new
            //    {
            //        m.Name,
            //        s.MachineId,
            //        s.EndTime
            //    };
            //var reuslt = await query.GroupBy(t => new{ t.MachineId,t.Name }, (key, g) => new {key.MachineId, List = g.ToList()})
            //    .Select(q => new
            //    {
            //        q.MachineId,
            //        EndTime = q.List.Max(t => t.EndTime)
            //    })
            //    .AnyAsync(s => s.EndTime >= today && s.EndTime <= expiryDate);
            //return reuslt;

            var today = DateTime.Now.ConvertTodayToCstTime();
            var expiryDate = today.AddDays(3);

            return await this.shiftEffectiveIntervalRepository.GetAll().AnyAsync(s => s.EndTime >= today && s.EndTime <= expiryDate);
        }

        public async Task DeleteByMachine(MachineShiftDeleteDto input)
        {
            var query = this.shiftEffectiveIntervalRepository.GetAll().Where(s => s.MachineId == input.MachineId).Where(s => !(s.EndTime < input.ShiftDay));

            foreach(var item in query)
            {
                if (item.StartTime>input.ShiftDay)
                {
                    await this.shiftEffectiveIntervalRepository.DeleteAsync(item);
                }
                else
                {
                    item.EndTime = input.ShiftDay;
                    await this.shiftEffectiveIntervalRepository.UpdateAsync(item);
                }
            }
        }
    }
}