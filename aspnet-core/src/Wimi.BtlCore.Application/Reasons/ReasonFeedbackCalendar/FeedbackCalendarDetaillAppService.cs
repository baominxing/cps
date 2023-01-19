using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Wimi.BtlCore;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Feedback;
using Wimi.BtlCore.Reasons.ReasonFeedbackCalendar.Dtos;
using WIMI.BTL.ReasonFeedbackCalendar.Dtos;

namespace WIMI.BTL.ReasonFeedbackCalendar
{
    public class FeedbackCalendarDetaillAppService : BtlCoreAppServiceBase, IFeedbackCalendarDetaillAppService
    {
        private readonly IRepository<FeedbackCalendarDetail> detailRepository;
        private readonly IRepository<Machine> machineRepository;

        public FeedbackCalendarDetaillAppService(IRepository<FeedbackCalendarDetail> detailRepository, IRepository<Machine> machineRepository)
        {
            this.detailRepository = detailRepository;
            this.machineRepository = machineRepository;
        }


        public async Task<PagedResultDto<NameValueDto>> ListMachines(SelectMachinesInputDto input)
        {
            var detaillMachineIds = await this.detailRepository.GetAll()
                .Where(d => d.FeedbackCalendarId == input.FeedbackCalendarId).Select(t => t.MachineId).ToListAsync();

            var query = this.machineRepository.GetAll()
                .Where(m => detaillMachineIds.All(g => g != m.Id))
                .Where(q => q.IsActive)
                .WhereIf(!input.Search.Value.IsNullOrWhiteSpace(), u => u.Name.Contains(input.Search.Value));

            var machineCount = await query.CountAsync();

            var machines = await query.OrderBy(u => u.Code).ToListAsync();

            var machineList = ObjectMapper.Map<IEnumerable<NameValueDto>>(machines.Select(u => u.ToNameValue())).ToList();
            return new DatatablesPagedResultOutput<NameValueDto>(machineCount, machineList, machineCount);
        }

        public async Task<PagedResultDto<SelectMachineRelatedDto>> ListSelectedMachines(SelectMachinesInputDto input)
        {
            var query = from pmr in this.detailRepository.GetAll()
                join m in this.machineRepository.GetAll() on pmr.MachineId equals m.Id
                where pmr.FeedbackCalendarId == input.FeedbackCalendarId
                select new SelectMachineRelatedDto
                {
                    Id = pmr.Id,
                    MachineCode = m.Code,
                    MachineName = m.Name,
                    CreationTime = pmr.CreationTime
                };

            var result = await query.OrderBy(input.Sorting).PageBy(input).AsNoTracking().ToListAsync();
            var resultCount = await query.CountAsync();
            return new DatatablesPagedResultOutput<SelectMachineRelatedDto>(
                resultCount,
                result,
                resultCount)
            {
                Draw = input.Draw
            };
        }

        public async Task AddMachinesToDetail(SelectMachineDto input)
        {
            if (!input.MachineIdList.Any())
            {
                return;
            }

            foreach (var item in input.MachineIdList)
            {
                await detailRepository.InsertAsync(new FeedbackCalendarDetail()
                {
                    FeedbackCalendarId = input.FeedbackCalendarId,
                    MachineId = item
                });
            }
        }

        public async Task Delete(EntityDto<int> input)
        {
            await this.detailRepository.DeleteAsync(input.Id);
        }
    }
}