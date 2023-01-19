using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Castle.Components.DictionaryAdapter;
using Castle.Core.Internal;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Plan.Manager;
using Wimi.BtlCore.Plan.ProcessPlans.Dtos;
using Wimi.BtlCore.Plan.Repository;

namespace Wimi.BtlCore.Plan.ProcessPlans
{
    public class ProcessPlanAppService : BtlCoreAppServiceBase, IProcessPlanAppService
    {
        private ProcessPlanManager processManager;
        private IRepository<DeviceGroup> devicegroupRepository;
        private IRepository<ProcessPlan> processPlanRepository;
        private IRepository<MachinesShiftDetail> shiftDetailRepository;
        private IRepository<ShiftSolution> shiftSolutionRepository;
        private IRepository<ShiftSolutionItem> shiftSolutionItemRepository;
        private IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private IRepository<PlanTarget> planTargetRepository;
        private readonly ISettingManager settingManager;
        private readonly IPlanRepository planRepository;

        public ProcessPlanAppService(
            ProcessPlanManager processManager,
            IRepository<DeviceGroup> devicegroupRepository,
            IRepository<ProcessPlan> processPlanRepository,
            IRepository<MachinesShiftDetail> shiftDetailRepository,
            IRepository<ShiftSolution> shiftSolutionRepository,
            IRepository<ShiftSolutionItem> shiftSolutionItemRepository,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            IRepository<PlanTarget> planTargetRepository,
            ISettingManager settingManager,
            IPlanRepository planRepository
            )
        {
            this.processManager = processManager;
            this.devicegroupRepository = devicegroupRepository;
            this.processPlanRepository = processPlanRepository;
            this.shiftDetailRepository = shiftDetailRepository;
            this.shiftSolutionItemRepository = shiftSolutionItemRepository;
            this.shiftSolutionRepository = shiftSolutionRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.planTargetRepository = planTargetRepository;
            this.settingManager = settingManager;
            this.planRepository = planRepository;
        }

        public async Task CreateProcessPlan(CreatePlanDto processPlan)
        {
            processPlan.ShiftTarget = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShiftItemDto>>(processPlan.ShiftTargetJson);
            var plan = ObjectMapper.Map<ProcessPlan>(processPlan);
            await processManager.CreateProcessPlan(plan);
        }

        public async Task UpdateProcessPlan(CreatePlanDto processPlan)
        {
            planRepository.DeletePlanTarget(processPlan.Id);
            processPlan.ShiftTarget = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShiftItemDto>>(processPlan.ShiftTargetJson);
            var plan = ObjectMapper.Map<ProcessPlan>(processPlan);
            await processManager.UpdateProcessPlan(plan);
        }

        public async Task UpdateProcessPlanState(EditPlanDto processPlan)
        {
            var plan = this.processPlanRepository.Get(processPlan.Id);
            plan.Status = processPlan.Status;

            plan.PauseTime = null;
            if (processPlan.Status == EnumPlanStatus.Complete)
            {
                plan.RealEndTime = DateTime.Now;
            }
            if (processPlan.Status == EnumPlanStatus.Pause)
            {
                plan.PauseTime = DateTime.Now;
            }
            await this.processPlanRepository.UpdateAsync(plan);
        }

        [HttpPost]
        public async Task DeleteProcessPlan(int id)
        {
            await processManager.DeleteProcessPlan(id);
        }

        public async Task<DatatablesPagedResultOutput<PlanOutputDto>> ListPlan(PlanInputDto input)
        {
            var query = from plan in this.processPlanRepository.GetAll()
                        join deviceGroup in this.devicegroupRepository.GetAll() on plan.DeviceGroupId equals deviceGroup.Id
                        select new PlanOutputDto()
                        {
                            PlanId = plan.Id,
                            PlanCode = plan.PlanCode,
                            PlanName = plan.PlanName,
                            PlanStatus = plan.Status,
                            PlanAmount = plan.PlanAmount,
                            TargetType = plan.TargetType,
                            YieldSummaryType = plan.YieldSummaryType,
                            RealStartTime = plan.RealStartTime,
                            RealEndTime = plan.RealEndTime,
                            PlanStartTime = plan.PlanStartTime,
                            PlanEndTime = plan.PlanEndTime,
                            MachineGroupName = deviceGroup.DisplayName,
                            ProductName = plan.ProductName,
                            ProcessAmount = plan.ProcessAmount,
                            TargetAmount = plan.TargetAmount
                        };
            bool isInitialTime;
            if (input.InitialStartTime == input.DateTimeFrom && input.InitialEndTime == input.DateTimeEnd)
            {
                isInitialTime = true;
            }
            else
            {
                isInitialTime = false;
            }

            if ((!input.ProductName.IsNullOrEmpty() || !input.PlanName.IsNullOrEmpty()) && isInitialTime)
            {
                query = query.WhereIf(!string.IsNullOrEmpty(input.ProductName), q => q.ProductName.Equals(input.ProductName))
                            .WhereIf(!string.IsNullOrEmpty(input.PlanName), q => q.PlanName.Contains(input.PlanName.Trim()))
                            .WhereIf(input.QueryStatus != null && input.QueryStatus.Count > 0,
                                q => input.QueryStatus.Contains(q.PlanStatus));
            }
            else if ((!input.ProductName.IsNullOrEmpty() || !input.PlanName.IsNullOrEmpty()) && !isInitialTime)
            {
                query = query.WhereIf(
                        input.DateTimeFrom.HasValue,
                        q => q.PlanStartTime >= input.DateTimeFrom
                             || (q.PlanStartTime == null && q.RealStartTime == null)
                             || (q.PlanStartTime == null && q.RealStartTime >= input.DateTimeFrom))
                    .WhereIf(
                        input.DateTimeEnd.HasValue,
                        q => q.PlanStartTime <= input.DateTimeEnd
                             || (q.PlanStartTime == null && q.RealStartTime == null)
                             || (q.PlanStartTime == null && q.RealStartTime <= input.DateTimeEnd))
                    .WhereIf(!string.IsNullOrEmpty(input.ProductName), q => q.ProductName.Equals(input.ProductName))
                    .WhereIf(!string.IsNullOrEmpty(input.PlanName), q => q.PlanName.Contains(input.PlanName.Trim()))
                    .WhereIf(input.QueryStatus != null && input.QueryStatus.Count > 0,
                        q => input.QueryStatus.Contains(q.PlanStatus));
            }
            else
            {
                query = query.WhereIf(
                        input.DateTimeFrom.HasValue,
                        q => q.PlanStartTime >= input.DateTimeFrom
                             || (q.PlanStartTime == null && q.RealStartTime == null)
                             || (q.PlanStartTime == null && q.RealStartTime >= input.DateTimeFrom))
                    .WhereIf(
                        input.DateTimeEnd.HasValue,
                        q => q.PlanStartTime <= input.DateTimeEnd
                             || (q.PlanStartTime == null && q.RealStartTime == null)
                             || (q.PlanStartTime == null && q.RealStartTime <= input.DateTimeEnd))
                    .WhereIf(input.QueryStatus != null && input.QueryStatus.Count > 0,
                        q => input.QueryStatus.Contains(q.PlanStatus));
            }

            var totalCount = await query.CountAsync();
            var filteredCount = totalCount;

            var pagedQuery = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new DatatablesPagedResultOutput<PlanOutputDto>(
                filteredCount,
                pagedQuery,
                totalCount,
                input.Draw);
        }

        [HttpPost]
        public async Task<EditPlanDto> GetPlan(NullableIdDto input)
        {
            if (!input.Id.HasValue)
            {
                return new EditPlanDto();
            }

            var targetId = input.Id.Value;
            var targetEntity = await this.processPlanRepository.GetAll().Include(p => p.ShiftTarget).FirstOrDefaultAsync(p => p.Id == targetId);

            if (targetEntity == null)
            {
                throw new UserFriendlyException(this.L("ThePlanDoesNotExistOrDeleted"));
            }
            var result = ObjectMapper.Map<EditPlanDto>(targetEntity);
            return result;
        }

        [HttpPost]
        public IEnumerable<EditPlanDto> GetPlanParameterByMachineId(int planId)
        {
            var plan = this.processPlanRepository.GetAll().Where(p => p.Id == planId).Select(p =>
                new EditPlanDto()
                {
                    YieldCounterMachineId = p.YieldCounterMachineId,
                    ProductId = p.ProductId,
                    DeviceGroupId = p.DeviceGroupId
                }).ToList();
            return plan;
        }

        [HttpPost]
        public async Task<List<ShiftItemDto>> GetCurrentShiftInfo(int deviceGroupId)
        {
            var currentDate = DateTime.Now.Date;
            var query = await ((from sd in shiftDetailRepository.GetAll()
                                join ss in shiftSolutionRepository.GetAll()
                                on sd.ShiftSolutionId equals ss.Id
                                join ssi in shiftSolutionItemRepository.GetAll()
                                on sd.ShiftSolutionItemId equals ssi.Id
                                join md in machineDeviceGroupRepository.GetAll()
                                on sd.MachineId equals md.MachineId
                                where sd.ShiftDay == currentDate
                                orderby ssi.CreationTime
                                select new
                                {
                                    ShiftId = sd.ShiftSolutionItemId,
                                    ShiftSolutionId = sd.ShiftSolutionItemId,
                                    ShiftName = ssi.Name,
                                    ShiftSolusionName = ss.Name,
                                    DeviceGroupId = md.DeviceGroupId,
                                    MachineId = md.MachineId
                                }).ToListAsync());
            if (query.Count > 0)
            {
                return query.Select(q => new ShiftItemDto() { ShiftId = q.ShiftId, ShiftName = q.ShiftName, SolutionId = q.ShiftSolutionId }).ToList();
            }
            return new EditableList<ShiftItemDto>();

        }

        [HttpPost]
        public string IsInProcessing(int id)
        {
            var targetPlan = this.processPlanRepository.Get(id);
            if (targetPlan.Status == EnumPlanStatus.InProgress)
            {
                return null;
            }
            var processingPlan = this.processPlanRepository.GetAll()
                .Where(p => p.DeviceGroupId == targetPlan.DeviceGroupId).ToList();
            foreach (var plan in processingPlan)
            {
                if (plan.Status == EnumPlanStatus.InProgress)
                {
                    return plan.PlanName;
                }
            }
            return null;
        }

        [HttpPost]
        public DatatablesPagedResultOutput<PlanOutputDto> GetPlanShiftItem(PlanInputDto input)
        {
            var plan = this.processPlanRepository.GetAll().Where(p => p.Id == input.Id).Include(p => p.ShiftTarget).FirstOrDefault();
            if (plan == null)
            {
                throw new UserFriendlyException(this.L("PlanAlreadyBeDeleted"));
            }

            var query = plan.ShiftTarget.Select(s => new PlanOutputDto()
            {
                ShiftName = s.ShiftName,
                ShiftTargetAmount = s.ShiftTargetAmount
            }).ToList();

            var totalCount = query.Count();
            var filteredCount = totalCount;

            var pagedQuery = query.OrderBy(s => s.ShiftName).ToList();

            return new DatatablesPagedResultOutput<PlanOutputDto>(
                filteredCount,
                pagedQuery,
                totalCount,
                input.Draw);
        }

        [HttpPost]
        public async Task<List<NameValueDto>> GetShiftSolutionName(ShiftSolutionNameInputDto input)
        {
            var returnValue = new List<NameValueDto>();

            var machineIds = await (from mdg in machineDeviceGroupRepository.GetAll()
                                    where mdg.DeviceGroupId == input.DeviceGroupId
                                    select mdg.MachineId).ToListAsync();

            returnValue = await planRepository.GetShiftSolutionName(machineIds, input.PlanStartTime);

            return returnValue;
        }

        [HttpPost]
        public async Task<List<ShiftItemDto>> GetShiftInfo(GetShiftInfoInputDto input)
        {
            var returnValue = new List<ShiftItemDto>();

            var query = await (from ssi in shiftSolutionItemRepository.GetAll()
                               join ss in shiftSolutionRepository.GetAll() on ssi.ShiftSolutionId equals ss.Id
                               where ss.Name == input.ShiftSolutionName
                               select new ShiftItemDto
                               {
                                   ShiftId = ssi.Id,
                                   SolutionId = ss.Id,
                                   ShiftName = ssi.Name
                               }).ToListAsync();


            //编辑
            if (input.PlanId > 0)
            {
                var shiftSolutionId = shiftSolutionRepository.FirstOrDefault(s => s.Name == input.ShiftSolutionName).Id;
                var plan = processPlanRepository.GetAll().Include(p => p.ShiftTarget).FirstOrDefault(p => p.Id == input.PlanId);
                var shiftItems = plan.ShiftTarget;

                if (shiftItems != null && shiftItems.Count() != 0)
                {
                    var item = shiftItems.First();

                    if (item.SolutionId == shiftSolutionId)
                    {
                        returnValue = ObjectMapper.Map<List<ShiftItemDto>>(shiftItems);
                    }
                    else
                    {
                        returnValue = query;
                    }
                }
                else
                {
                    returnValue = query;
                }
            }
            else
            {
                returnValue = query;
            }

            return returnValue;
        }
    }
}
