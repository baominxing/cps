using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Cutter;
using Wimi.BtlCore.BasicData.Machines;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.ThirdpartyApis.Interfaces;
using Wimi.BtlCore.ThirdpartyApis.Dto;

namespace Wimi.BtlCore.ThirdpartyApis.Managers
{
    public class CutterComponentManager : BtlCoreDomainServiceBase, ICutterComponentManager
    {
        private readonly IRepository<CutterStates> cutterStateRepository;
        private readonly IRepository<Machine> machineRepository;

        public CutterComponentManager(IRepository<CutterStates> cutterStateRepository,
            IRepository<Machine> machineRepository)
        {
            this.cutterStateRepository = cutterStateRepository;
            this.machineRepository = machineRepository;
        }

        public async Task<ApiResponseObject> ListToolWarnings()
        {
            var query = await (from c in this.cutterStateRepository.GetAll()
                join m in this.machineRepository.GetAll() on c.MachineId equals m.Id
                where c.CutterLifeStatus != EnumCutterLifeStates.Normal
                select new { cutter = c, machine = m }).ToListAsync();         

            var cutters = query.Select(
                c => new ToolWarningDto()
                {
                    MachineId = c.machine.Id,
                    MachineName = c.machine.Name,
                    CutterNo = c.cutter.CutterNo,
                    CutterTValue = c.cutter.CutterTValue,
                    CountingMethod = this.L(c.cutter.CountingMethod.ToString()),
                    CutterLifeStatus = this.L(c.cutter.CutterLifeStatus.ToString()),
                    CutterUsedStatus = this.L(c.cutter.CutterUsedStatus.ToString()),
                    OriginalLife = c.cutter.OriginalLife,
                    RestLife = c.cutter.RestLife,
                    UsedLife = c.cutter.UsedLife,
                    WarningLife = c.cutter.WarningLife
                });

            var series = typeof(ToolWarningDto).GetProperties().Select(p => p.Name).ToList();
            var data = new List<IEnumerable<dynamic>>();

            foreach(var c in cutters)
            {
                var values = new List<dynamic>();
                series.ForEach(s => values.Add(c.GetPropertyValue<dynamic>(s)));
                data.Add(values);
            }

            return new ApiResponseObject(ApiItemType.Objects, ApiTargetType.None)
            {
                FixedSeries = true,
                Data = data,
                Series = new List<IEnumerable<dynamic>>() { series.Select(this.L) }
            };
        }
    }
}