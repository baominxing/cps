using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Authorization.DmpMachineSetting.Dto;
using Wimi.BtlCore.Dmps;
using Wimi.BtlCore.Dto;
using Abp.Collections.Extensions;

namespace Wimi.BtlCore.Authorization.DmpMachineSetting
{
    public class DmpMachineAppService : BtlCoreAppServiceBase, IDmpMachineAppService
    {
        private readonly IRepository<Dmp> dmpRepository;
        private readonly IRepository<DmpMachine> dmpMachineRepository;
        private readonly IRepository<BasicData.Machines.Machine> machineRepository;

        public DmpMachineAppService(IRepository<Dmp> dmpRepository,
            IRepository<DmpMachine> dmpMachineRepository,
            IRepository<BasicData.Machines.Machine> machineRepository)
        {
            this.dmpRepository = dmpRepository;
            this.dmpMachineRepository = dmpMachineRepository;
            this.machineRepository = machineRepository;
        }

        [HttpPost]
        public async Task<ListResultDto<DmpDto>> GetDmps()
        {
            var returnValue = new ListResultDto<DmpDto>();
            var dmps = await dmpRepository.GetAll().ToListAsync();

            return new ListResultDto<DmpDto>(
                dmps.Select(
                        dmp =>
                        {
                            var dto = new DmpDto();
                            dto.Id = dmp.Id;
                            dto.Code = dmp.ServiceCode;
                            dto.CreationTime = dmp.CreationTime;
                            dto.DisplayName = dmp.IpAdress + ":" + dmp.WebPort;
                            dto.MemberCount = GetMemberCount(dmp.Id);
                            return dto;
                        })
                    .ToList());
        }

        private int GetMemberCount(int dmpId)
        {
            return dmpMachineRepository.GetAll().Where(d => d.DmpId == dmpId).Count();
        }

        [HttpPost]
        public async Task<PagedResultDto<GetDmpMachinesOutputDto>> GetDmpMachines(GetDmpMachinesInputDto input)
        {
            var query = from uou in dmpMachineRepository.GetAll()
                        join ou in dmpRepository.GetAll() on uou.DmpId equals ou.Id
                        join m in machineRepository.GetAll() on uou.MachineId equals m.Id
                        where uou.DmpId == input.Id
                        select new GetDmpMachinesOutputDto
                        {
                            Id = m.Id,
                            Code = m.Code,
                            Name = m.Name,
                            Desc = m.Desc,
                            SortSeq = m.SortSeq,
                            AddedTime = uou.CreationTime
                        };

            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(input.Sorting).AsNoTracking().PageBy(input).ToListAsync();

            return new DatatablesPagedResultOutput<GetDmpMachinesOutputDto>(totalCount, items);
        }

        public async Task<string> AddMachineListToDmp(MachineListToDmpInputDto input)
        {
            List<int> addedMachineIdList = new List<int>();
            List<string> addedMachineNameList = new List<string>();
            string message = null;
            foreach (var machineId in input.MachineIdList)
            {
                var alreadyAdded = await this.dmpMachineRepository.GetAll().AnyAsync(m => m.MachineId == machineId);

                if (!alreadyAdded)
                {
                    await dmpMachineRepository.InsertAsync(new DmpMachine(machineId, input.DmpId));
                }
                else
                {
                    addedMachineIdList.Add(machineId);
                }
            }

            var machines = await this.machineRepository.GetAll().Where(m => input.MachineIdList.Contains(m.Id))
                .ToListAsync();

            foreach (var id in addedMachineIdList)
            {
                var machine = machines.FirstOrDefault(m => m.Id == id);
                if (machine != null) addedMachineNameList.Add(machine.Name);
                message = this.L("MachinesHasBeenInOtherDmp{0}", addedMachineNameList.JoinAsString(","));
            }

            return message;
        }

        public async Task RemoveMachineFromDmp(RemoveMachineFromDmpInputDto input)
        {
            await this.dmpMachineRepository.DeleteAsync(x=>x.DmpId == input.DmpId && x.MachineId == input.MachineId);
        }

        public async Task BatchRemoveMachineFromDmp(BatchRemoveMachineFromDmpInputDto input)
        {
            await this.dmpMachineRepository.DeleteAsync(x => x.DmpId == input.DmpId && input.MachineIds.Contains(x.MachineId));
        }
    }
}
