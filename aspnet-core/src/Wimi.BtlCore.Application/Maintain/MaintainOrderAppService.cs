using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.MachineTypes;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Maintain.Dto;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;

namespace Wimi.BtlCore.Maintain
{
    public class MaintainOrderAppService : BtlCoreAppServiceBase, IMaintainOrderAppService
    {
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<MachineType> machineTypeRepository;
        private readonly IRepository<User, long> userRepository;
        private readonly IRepository<MaintainPlan> maintainPlanRepository;
        private readonly IRepository<MaintainOrder> maintainOrderRepository;

        public MaintainOrderAppService(
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

        [HttpPost]
        public MaintainOrderDto GetMaintainOrder(MaintainOrderInputDto input)
        {
            var query = from mp in this.maintainPlanRepository.GetAll().AsNoTracking()
                        join mo in this.maintainOrderRepository.GetAll().AsNoTracking() on mp.Code equals mo.MaintainPlanCode
                        join m in this.machineRepository.GetAll().AsNoTracking() on mp.MachineId equals m.Id
                        join mt in this.machineTypeRepository.GetAll() on m.MachineTypeId equals mt.Id
                        join mpu in this.userRepository.GetAll().AsNoTracking() on mp.PersonInChargeId equals mpu.Id
                        join mou in this.userRepository.GetAll().AsNoTracking() on mo.MaintainUserId equals mou.Id into defaultuser
                        from dmou in defaultuser.DefaultIfEmpty()
                        where mo.Id == input.Id
                        select new MaintainOrderDto()
                        {
                            Id = mo.Id,
                            MaintainPlanCode = mp.Code,
                            MaintainPlanName = mp.Name,
                            StartDate = mp.StartDate,
                            EndDate = mp.EndDate,
                            IntervalDate = mp.IntervalDate,
                            PersonInChargeName = mpu.Name,
                            Code = mo.Code,
                            MachineId = mp.MachineId,
                            MachineCode = m.Code,
                            MachineName = m.Name,
                            MachineType = mt.Name,
                            Status = mo.Status == EnumMaintainOrderStatus.Done ? mo.Status.ToString() : mo.ScheduledDate < DateTime.Today ? EnumMaintainOrderStatus.Over.ToString() : EnumMaintainOrderStatus.Undo.ToString(),
                            ScheduledDate = mo.ScheduledDate,
                            StartTime = mo.StartTime,
                            EndTime = mo.EndTime,
                            Cost = mo.Cost,
                            MaintainUserId = dmou == null ? 0 : (int)dmou.Id,
                            MaintainUserName = dmou.Name,
                            MaintainPlanMemo = mp.Memo,
                            Memo = mo.Memo
                        };
            return query.FirstOrDefault();
        }

        public async Task<DatatablesPagedResultOutput<MaintainOrderDto>> ListMaintainOrder(MaintainOrderInputDto input)
        {
            var query = from mp in this.maintainPlanRepository.GetAll().AsNoTracking()
                        join mo in this.maintainOrderRepository.GetAll().AsNoTracking() on mp.Code equals mo.MaintainPlanCode
                        join m in this.machineRepository.GetAll().AsNoTracking() on mp.MachineId equals m.Id
                        join mt in this.machineTypeRepository.GetAll() on m.MachineTypeId equals mt.Id
                        join u in this.userRepository.GetAll().AsNoTracking() on mo.MaintainUserId equals u.Id into defaultuser
                        from du in defaultuser.DefaultIfEmpty()
                        select new MaintainOrderDto()
                        {
                            Id = mo.Id,
                            MaintainPlanCode = mp.Code,
                            Code = mo.Code,
                            MachineId = mp.MachineId,
                            MachineCode = m.Code,
                            MachineName = m.Name,
                            MachineType = mt.Name,
                            Status = mo.Status == EnumMaintainOrderStatus.Done ? mo.Status.ToString() : mo.ScheduledDate < DateTime.Today ? EnumMaintainOrderStatus.Over.ToString() : EnumMaintainOrderStatus.Undo.ToString(),
                            EnumStatus = mo.Status == EnumMaintainOrderStatus.Done ? mo.Status : mo.ScheduledDate < DateTime.Today ? EnumMaintainOrderStatus.Over : EnumMaintainOrderStatus.Undo,
                            ScheduledDate = mo.ScheduledDate,
                            StartTime = mo.StartTime,
                            EndTime = mo.EndTime,
                            Cost = mo.Cost,
                            MaintainUserName = du.Name
                        };


            query = query.WhereIf(
                 !input.Code.IsNullOrEmpty(),
                c => c.Code.ToLower().Contains(input.Code.ToLower().Trim()));

            query = query.WhereIf(
                 !input.MaintainPlanCode.IsNullOrEmpty(),
                c => c.MaintainPlanCode.ToLower().Contains(input.MaintainPlanCode.ToLower().Trim()));

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

            return new DatatablesPagedResultOutput<MaintainOrderDto>(
                filteredCount,
                pagedQuery,
                totalCount,
                input.Draw);
        }

        [HttpPost]
        public async Task Update(MaintainOrderInputDto input)
        {
            if (IsOnlyOne(input))
            {
                var plan = this.maintainPlanRepository.FirstOrDefault(s => s.Code == input.MaintainPlanCode);
                plan.Status = EnumMaintainPlanStatus.Done;
                this.maintainPlanRepository.Update(plan);
            }
            else if (IsFirstOrder(input))
            {
                var plan = this.maintainPlanRepository.FirstOrDefault(s => s.Code == input.MaintainPlanCode);
                plan.Status = EnumMaintainPlanStatus.Processing;
                this.maintainPlanRepository.Update(plan);
            }
            else if (IsLastOrder(input))
            {
                var plan = this.maintainPlanRepository.FirstOrDefault(s => s.Code == input.MaintainPlanCode);
                plan.Status = EnumMaintainPlanStatus.Done;
                this.maintainPlanRepository.Update(plan);
            }

            var order = this.maintainOrderRepository.Get(input.Id);
            order.StartTime = input.StartTime;
            order.EndTime = input.EndTime;
            order.Status = EnumMaintainOrderStatus.Done;
            order.Cost = Convert.ToDecimal((((DateTime)input.EndTime - (DateTime)input.StartTime)).TotalHours);
            order.MaintainUserId = input.MaintainUserId ?? 0;
            order.Memo = input.Memo;
            await this.maintainOrderRepository.UpdateAsync(order);
        }
        private bool IsOnlyOne(MaintainOrderInputDto input)
        {
            var count = maintainOrderRepository.GetAll().Count(s => s.MaintainPlanCode == input.MaintainPlanCode);
            return count == 1;
        }
        private bool IsLastOrder(MaintainOrderInputDto input)
        {
            return this.maintainOrderRepository.GetAll().OrderByDescending(s => s.ScheduledDate).FirstOrDefault(s => s.MaintainPlanCode == input.MaintainPlanCode).ScheduledDate == input.ScheduledDate;
        }

        private bool IsFirstOrder(MaintainOrderInputDto input)
        {
            return this.maintainOrderRepository.GetAll().OrderBy(s => s.ScheduledDate).FirstOrDefault(s => s.MaintainPlanCode == input.MaintainPlanCode).ScheduledDate == input.ScheduledDate;
        }
    }
}
