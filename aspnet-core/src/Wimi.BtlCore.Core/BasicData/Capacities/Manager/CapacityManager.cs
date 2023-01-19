using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Wimi.BtlCore.BasicData.Capacities.Manager
{
    public class CapacityManager : BtlCoreDomainServiceBase
    {
        private readonly IRepository<Capacity> capacityRepository;

        public CapacityManager(IRepository<Capacity> capacityRepository)
        {
            this.capacityRepository = capacityRepository;
        }

        public IEnumerable<Capacity> GetLastCapacityRecord(string machineCode, int limitNum)
        {
            var query = this.capacityRepository.GetAll()
                .Where(x => x.MachineCode == machineCode)
                .OrderByDescending(x => x.StartTime)
                .Take(limitNum);

            var result = query.ToList();
            return result;
        }

        public Capacity GetFirstCapacityRecordByDay(string machineCode)
        {
            var currentDayFirstCreationTime = DateTime.Now.Date;

            var query = this.capacityRepository.GetAll()
                .Where(x => x.MachineCode == machineCode && x.StartTime <= currentDayFirstCreationTime)
                .OrderByDescending(x => x.StartTime)
                .Take(1);


            var result = query.FirstOrDefault();
            return result;
        }
    }
}
