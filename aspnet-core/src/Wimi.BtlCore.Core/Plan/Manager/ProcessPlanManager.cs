using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Plan.Manager
{
    public class ProcessPlanManager : BtlCoreDomainServiceBase
    {
        private IRepository<ProcessPlan> processPlanRepository;
        private IRepository<PlanTarget> planTargetRepository;
        public  ProcessPlanManager(IRepository<ProcessPlan> processPlanRepository, IRepository<PlanTarget> planTargetRepository)
        {
            this.processPlanRepository = processPlanRepository;
            this.planTargetRepository = planTargetRepository;
        }

        public async Task CreateProcessPlan(ProcessPlan plan)
        {
            plan.Verify();
            ValidatePlan(plan);
            IsTimeAvailable(plan);
            await processPlanRepository.InsertAsync(plan);
        }
        public async Task DeleteProcessPlan(int processPlanId)
        {
            var plan =await processPlanRepository.GetAll().Include(pp=>pp.ShiftTarget).FirstOrDefaultAsync(pp => pp.Id == processPlanId);
            if (plan==null)
            {
                throw new UserFriendlyException(this.L("PlanAlreadyBeDeleted"));
            }
            else if(!plan.CanDelete())
            {
                throw new UserFriendlyException(this.L("TheCurrentStatePlansCannotBeDeleted"));
            }  
            await processPlanRepository.DeleteAsync(plan);
           
        }

        public async Task UpdateProcessPlan(ProcessPlan processPlan)
        {
            processPlan.Verify();
            var plan = await processPlanRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(pp => pp.Id == processPlan.Id);
            if (plan == null)
            {
                throw new UserFriendlyException(this.L("PlanAlreadyBeDeleted"));
            }
            //校验状态
            if (!processPlan.CanUpdateStatus(processPlan.Status))
            {
                throw new UserFriendlyException(this.L("CannotBeUpdatedToTheTargetState"));
            }

            if (plan.PlanStartTime != processPlan.PlanStartTime)
            {
                IsTimeAvailable(processPlan);
            }
            ValidatePlan(processPlan);

            //已经实际开始了的计划并且改了目标量维度
            if (plan.RealStartTime != null && plan.TargetType != processPlan.TargetType)
            {
                //计划切分,目标量维度变化，保持原来的不变
                processPlan.Status = plan.Status;
                processPlan.PlanCode = plan.PlanCode;
                plan.RealEndTime = DateTime.Now;
                plan.Status = EnumPlanStatus.AutoComplete;
                if (plan.PlanAmount == processPlan.PlanAmount)
                {
                    processPlan.PlanAmount = plan.PlanAmount - plan.ProcessAmount;
                }
                plan.PlanAmount = plan.ProcessAmount;
                await this.processPlanRepository.UpdateAsync(plan);
                var insertPlanEntity = new ProcessPlan() 
                { 
                    PlanName=processPlan.PlanName,
                    PlanCode=processPlan.PlanCode,
                    ProductId=processPlan.ProductId,
                    ProductName= processPlan.ProductName,
                    PlanAmount= processPlan.PlanAmount,
                    DeviceGroupId = processPlan.DeviceGroupId,
                    IsTimeRangeSelect = processPlan.IsTimeRangeSelect,
                    PlanStartTime = processPlan.PlanStartTime,
                    PlanEndTime = processPlan.PlanEndTime,
                    RealStartTime = processPlan.RealStartTime,
                    RealEndTime = processPlan.RealEndTime,
                    PauseTime = processPlan.PauseTime,
                    IsAutoFinishCurrentPlan = processPlan.IsAutoFinishCurrentPlan,
                    IsAutoStartNextPlan = processPlan.IsAutoStartNextPlan,
                    TargetType = processPlan.TargetType,
                    TargetAmount = processPlan.TargetAmount,
                    ShiftTarget = processPlan.ShiftTarget,
                    YieldSummaryType = processPlan.YieldSummaryType,
                    YieldCounterMachineId = processPlan.YieldCounterMachineId,
                    Status = processPlan.Status,
                    ProcessAmount = processPlan.ProcessAmount
                };
                await this.processPlanRepository.InsertAsync(insertPlanEntity);
                
            }
            else
            {
                foreach (var shiftTarget in processPlan.ShiftTarget)
                {
                    await this.planTargetRepository.InsertAsync(shiftTarget);
                }
                processPlan.ProcessAmount = plan.ProcessAmount;
                processPlan.Status = plan.Status;
                processPlan.PlanCode = plan.PlanCode;
                processPlan.RealStartTime = plan.RealStartTime;
                await this.processPlanRepository.UpdateAsync(processPlan);
            }

        }

        private void IsTimeAvailable(ProcessPlan plan)
        {
            //校验时间
            if (plan.IsTimeRangeSelect)
            {
                if (plan.PlanStartTime.HasValue && plan.PlanEndTime.HasValue)
                {
                    if (plan.PlanStartTime.Value < DateTime.Now)
                    {
                        throw new UserFriendlyException(this.L("PlanDateTimeCompare"));
                    }

                    if (plan.PlanEndTime.Value <= plan.PlanStartTime.Value)
                    {
                        throw new UserFriendlyException(this.L("PlanEndDateTimeCompare"));
                    }
                }
            }
        }
       
        private void ValidatePlan(ProcessPlan plan)
        {
            //校验计划名称是否重复
            var query = processPlanRepository.GetAll().Where(p=>p.Status!=EnumPlanStatus.AutoComplete&&p.Id!=plan.Id);
            if (query.Any(pp => pp.PlanName == plan.PlanName && pp.PlanCode != plan.PlanCode))
            {
                throw new UserFriendlyException(this.L("PlanNameCannotBeDuplicated"));
            }
            //1.新增、暂停、进行中的计划按照  计划结束时间校验
            //2.结束的计划按照    实际结束时间校验
            if (plan.IsTimeRangeSelect)
            {
                var plans = query.Where(pp => pp.DeviceGroupId == plan.DeviceGroupId && (pp.PlanEndTime.Value > DateTime.Now)).ToList();
                if (plans.Count > 0)
                {
                    if (plans.Any(p =>
                    {
                        switch (p.Status)
                        {
                            case EnumPlanStatus.New:
                            case EnumPlanStatus.Pause:
                            case EnumPlanStatus.InProgress:
                                if (p.RealEndTime.HasValue)
                                {
                                    if (!(p.PlanStartTime > plan.PlanEndTime || p.RealEndTime < plan.PlanStartTime))
                                        return true;//时间重复
                                    return false; //时间没有重复
                                }
                                else
                                {
                                    if (!(p.PlanStartTime > plan.PlanEndTime || p.PlanEndTime < plan.PlanStartTime))
                                        return true;//时间重复
                                    return false; //时间没有重复
                                }
                              
                            case EnumPlanStatus.Complete:
                                if (plan.RealStartTime.HasValue)
                                {
                                    if (p.PlanEndTime> plan.RealStartTime.Value&&p.PlanStartTime<plan.RealStartTime)
                                        return true;
                                   
                                }
                                if (plan.RealEndTime.HasValue)
                                {
                                    if (p.PlanEndTime > plan.RealEndTime .Value&&p.PlanStartTime<plan.RealEndTime.Value)
                                        return true;
                                }
                                if (plan.RealEndTime == null && plan.RealStartTime == null)
                                {
                                    if (!(p.PlanStartTime > plan.PlanEndTime || p.PlanEndTime < plan.PlanStartTime))
                                        return true;//时间重复
                                }

                                return false;
                            default: return true;
                        }

                    }))
                    {
                        throw new UserFriendlyException(this.L("OverlappingOfScheduledTimeFrames"));
                    }
                }
            }
        }

        public async Task DeletePlansByDeviceGroupId(int deviceGroupId)
        {
            await this.processPlanRepository.DeleteAsync(pp=>pp.DeviceGroupId == deviceGroupId);
        }
     
    }
}
