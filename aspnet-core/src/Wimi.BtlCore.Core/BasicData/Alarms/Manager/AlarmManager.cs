using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Wimi.BtlCore.BasicData.Alarms.Manager
{
    public class AlarmManager : BtlCoreDomainServiceBase
    {
        private readonly IRepository<Alarm,long> alarmRepository;

        public AlarmManager(IRepository<Alarm, long> alarmRepository)
        {
            this.alarmRepository = alarmRepository;
        }

        public IEnumerable<Alarm> GetMachineHistoryAlarm(string machineCode, int pageNo, int pageSize)
        {
            var skipNum = (pageNo - 1) * pageSize;
            var limitNum = pageSize;

            var query = this.alarmRepository.GetAll()
                .Where(x => x.MachineCode == machineCode)
                .OrderByDescending(x=>x.StartTime)
                .Skip(skipNum)
                .Take(limitNum);

            var docList = query.ToList();

            return docList;
        }
    }
}
