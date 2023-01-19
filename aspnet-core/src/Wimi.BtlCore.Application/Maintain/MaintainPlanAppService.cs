using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.MachineTypes;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Maintain.Dto;

namespace Wimi.BtlCore.Maintain
{
    public class MaintainPlanAppService : BtlCoreAppServiceBase, IMaintainPlanAppService
    {
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<MachineType> machineTypeRepository;
        private readonly IRepository<User, long> userRepository;
        private readonly IRepository<MaintainPlan> maintainPlanRepository;
        private readonly IRepository<MaintainOrder> maintainOrderRepository;

        public MaintainPlanAppService(
            IRepository<Machine> machineRepository,
            IRepository<MachineType> machineTypeRepository,
            IRepository<User, long> userRepository,
            IRepository<MaintainPlan> maintainPlanRepository,
            IRepository<MaintainOrder> maintainOrderRepository)
        {
            this.machineRepository = machineRepository;
            this.machineTypeRepository = machineTypeRepository;
            this.userRepository = userRepository;
            this.maintainPlanRepository = maintainPlanRepository;
            this.maintainOrderRepository = maintainOrderRepository;
        }

        public async Task<DatatablesPagedResultOutput<MaintainPlanDto>> ListMaintainPlan(MaintainPlanInputDto input)
        {
            var query = from mp in this.maintainPlanRepository.GetAll().AsNoTracking()
                        join m in this.machineRepository.GetAll().AsNoTracking() on mp.MachineId equals m.Id
                        join mt in this.machineTypeRepository.GetAll() on m.MachineTypeId equals mt.Id
                        join u in this.userRepository.GetAll().AsNoTracking() on mp.PersonInChargeId equals u.Id
                        select new MaintainPlanDto()
                        {
                            Id = mp.Id,
                            Code = mp.Code,
                            Name = mp.Name,
                            MachineId = mp.MachineId,
                            MachineCode = m.Code,
                            MachineName = m.Name,
                            MachineType = mt.Name,
                            Status = mp.Status.ToString(),
                            EnumStatus = mp.Status,
                            StartDate = mp.StartDate,
                            EndDate = mp.EndDate,
                            IntervalDate = mp.IntervalDate,
                            PersonInChargeId = mp.PersonInChargeId,
                            PersonInChargeName = u.Name,
                            Memo = mp.Memo
                        };

            query = query.WhereIf(
                 !input.Code.IsNullOrEmpty(),
                c => c.Code.ToLower().Contains(input.Code.ToLower().Trim()));

            query = query.WhereIf(
                 !input.Name.IsNullOrEmpty(),
                c => c.Name.ToLower().Contains(input.Name.ToLower().Trim()));

            query = query.WhereIf(
                input.MachineId != 0,
               c => c.MachineId == input.MachineId);

            if (input.StatusList.Any())
            {
                query = query.Where(q => input.StatusList.Contains(q.EnumStatus));
            }

            var totalCount = await query.CountAsync();
            var filteredCount = totalCount;

            var pagedQuery = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new DatatablesPagedResultOutput<MaintainPlanDto>(
                filteredCount,
                pagedQuery,
                totalCount,
                input.Draw);
        }

        public async Task Create(MaintainPlanInputDto input)
        {
            if (DateRangeIsNotValidated(input))
            {
                throw new UserFriendlyException(this.L("FailureToGenerateValidPlans"));
            }

            if (MaintainPlanNameIsDuplicated(input))
            {
                throw new UserFriendlyException(this.L("PlanNameAlreadyExist"));
            }

            if (input.GenerateNewMaintainCode)
            {
                input.Code = this.GenerateNewMaintainPlanCode();
            }

            var entity = ObjectMapper.Map<MaintainPlan>(input);

            entity.Status = EnumMaintainPlanStatus.New;
            entity.Id = 0;

            await this.maintainPlanRepository.InsertAsync(entity);

            await this.GenerateMaintainOrder(input);
        }

        [HttpPost]
        public async Task Delete(MaintainPlanInputDto input)
        {
            var maintainPlan = this.maintainPlanRepository.Get(input.Id);

            if (maintainPlan.Status != EnumMaintainPlanStatus.New)
            {
                throw new UserFriendlyException(this.L("OnlyCanDeleteMaintenancePlans"));
            }

            await this.maintainPlanRepository.DeleteAsync(maintainPlan);

            await this.maintainOrderRepository.DeleteAsync(s => s.MaintainPlanCode == maintainPlan.Code);
        }

        [HttpPost]
        public MaintainPlanDto GetMaintainPlan(MaintainPlanInputDto input)
        {
            var maintainPlan = (from mp in this.maintainPlanRepository.GetAll().AsNoTracking()
                                join m in this.machineRepository.GetAll().AsNoTracking() on mp.MachineId equals m.Id
                                join mt in this.machineTypeRepository.GetAll() on m.MachineTypeId equals mt.Id
                                join u in this.userRepository.GetAll().AsNoTracking() on mp.PersonInChargeId equals u.Id
                                where mp.Id == input.Id
                                select new MaintainPlanDto()
                                {
                                    Id = mp.Id,
                                    Code = mp.Code,
                                    Name = mp.Name,
                                    MachineId = mp.MachineId,
                                    MachineCode = m.Code,
                                    MachineName = m.Name,
                                    MachineType = mt.Name,
                                    Status = mp.Status.ToString(),
                                    StartDate = mp.StartDate,
                                    EndDate = mp.EndDate,
                                    IntervalDate = mp.IntervalDate,
                                    PersonInChargeId = mp.PersonInChargeId,
                                    PersonInChargeName = u.Name,
                                    Memo = mp.Memo
                                }).FirstOrDefault();

            return maintainPlan;
        }

        [HttpPost]
        public async Task Update(MaintainPlanInputDto input)
        {
            switch (input.Status)
            {
                case EnumMaintainPlanStatus.New:
                    await this.Delete(input);

                    input.GenerateNewMaintainCode = false;

                    await this.Create(input);
                    break;
                case EnumMaintainPlanStatus.Processing:
                    await this.UpdateProcessingMaintainPlan(input);
                    break;
                case EnumMaintainPlanStatus.Done:
                    throw new UserFriendlyException(this.L("OnlyCanSavePlanInProcess"));
            }
        }

        private async Task UpdateProcessingMaintainPlan(MaintainPlanInputDto input)
        {
            var originalPlan = await this.maintainPlanRepository.FirstOrDefaultAsync(s => s.Id == input.Id);

            if (originalPlan == null)
            {
                throw new UserFriendlyException(this.L("PlanNotExist"));
            }

            if (input.EndDate < originalPlan.EndDate)
            {
                await this.maintainOrderRepository.DeleteAsync(s => s.MaintainPlanCode == input.Code && s.ScheduledDate > input.EndDate && s.Status != EnumMaintainOrderStatus.Done);
            }
            else if (input.EndDate > originalPlan.EndDate)
            {
                var lastOrder = await this.maintainOrderRepository.GetAll().Where(s => s.MaintainPlanCode == input.Code).AsNoTracking().OrderByDescending(s => s.ScheduledDate).FirstOrDefaultAsync();
                input.StartDate = lastOrder.ScheduledDate.AddDays(input.IntervalDate);
                await this.GenerateMaintainOrder(input);
            }
            originalPlan.Name = input.Name;
            originalPlan.EndDate = input.EndDate;

            await this.maintainPlanRepository.UpdateAsync(originalPlan);
        }

        private string GenerateNewMaintainPlanCode()
        {
            return "MP" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        private string GenerateNewMaintainOrderCode()
        {
            return "MO" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        private bool MaintainPlanNameIsDuplicated(MaintainPlanInputDto input)
        {
            return this.maintainPlanRepository.GetAll().Any(s => s.Name == input.Name && s.Id != input.Id);
        }

        private bool DateRangeIsNotValidated(MaintainPlanInputDto input)
        {
            return (input.EndDate.AddDays(1) - input.StartDate).TotalDays < Convert.ToDouble(input.IntervalDate);
        }

        private async Task GenerateMaintainOrder(MaintainPlanInputDto input)
        {
            for (var st = input.StartDate; st <= input.EndDate; st = st.AddDays(Convert.ToInt32(input.IntervalDate)))
            {
                var maintainOrder = new MaintainOrder();
                maintainOrder.MaintainPlanCode = input.Code;
                maintainOrder.Code = GenerateNewMaintainOrderCode();
                maintainOrder.MachineId = input.MachineId;
                maintainOrder.ScheduledDate = st;
                maintainOrder.Status = EnumMaintainOrderStatus.Undo;
                maintainOrder.Memo = string.Empty;

                await Task.Delay(100);

                await this.maintainOrderRepository.InsertAsync(maintainOrder);
            }
        }

        public Task StopMaintainPlan(MaintainPlanInputDto input)
        {
            throw new NotImplementedException();
        }
    }
}
