using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Order.Crafts;
using Wimi.BtlCore.Order.DefectiveReasons;
using Wimi.BtlCore.Order.LoginReports.Dtos;
using Wimi.BtlCore.Order.Products;
using Wimi.BtlCore.Order.StandardTimes;
using Wimi.BtlCore.Order.WorkOrders;

namespace Wimi.BtlCore.Order.LoginReports
{
    [AbpAuthorize(PermissionNames.Pages_Order_LoginReport)]
    public class LoginReportAppService : BtlCoreAppServiceBase, ILoginReportAppService
    {
        private readonly IRepository<CraftProcess> craftProcessRepository;

        private readonly IRepository<Craft> craftRepository;

        private readonly IRepository<DefectiveReason> defectiveRepository;

        private readonly IRepository<Machine> machineRepository;

        private readonly IRepository<Products.Product> productRepository;

        private readonly IRepository<StandardTime> standardTimeRepository;

        private readonly IRepository<WorkOrderDefectiveRecords> workOrderDefectiveRecordsRepository;

        private readonly WorkOrderManager workOrderManager;

        private readonly IRepository<WorkOrder> workOrdersRepository;

        private readonly WorkOrderTaskManager workOrderTaskManager;

        private readonly IRepository<WorkOrderTasks> workOrderTaskRepository;

        public LoginReportAppService(
            IRepository<WorkOrder> workOrdersRepository,
            IRepository<WorkOrderTasks> workOrderTaskRepository,
            IRepository<WorkOrderDefectiveRecords> workOrderDefectiveRecordsRepository,
            IRepository<DefectiveReason> defectiveRepository,
            IRepository<Machine> machineRepository,
            IRepository<Products.Product> productRepository,
            IRepository<Craft> craftRepository,
            IRepository<CraftProcess> craftProcessRepository,
            IRepository<StandardTime> standardTimeRepository,
            WorkOrderManager workOrderManager,
            WorkOrderTaskManager workOrderTaskManager)
        {
            this.workOrdersRepository = workOrdersRepository;
            this.workOrderTaskRepository = workOrderTaskRepository;
            this.workOrderDefectiveRecordsRepository = workOrderDefectiveRecordsRepository;
            this.defectiveRepository = defectiveRepository;
            this.machineRepository = machineRepository;
            this.productRepository = productRepository;
            this.craftRepository = craftRepository;
            this.craftProcessRepository = craftProcessRepository;
            this.standardTimeRepository = standardTimeRepository;
            this.workOrderManager = workOrderManager;
            this.workOrderTaskManager = workOrderTaskManager;
        }

        /// <summary>
        /// 关闭工单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Close(EntityDto input)
        {
            await this.workOrderManager.Close(input.Id);

            // 将该工单从已登录的设备上移除
            this.workOrderTaskManager.UpdateWorkOrderTaskEndTime(input.Id);
        }

        /// <summary>
        /// 获取次品原因及该原因下已生产的个数，未报工时个数为0。
        /// </summary>
        /// <param name="input">工单任务Id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MachineReportDto> GetDefectiveReasonsForMachineReport(EntityDto input)
        {
            var machineReportQuery = from wt in this.workOrderTaskRepository.GetAll()
                                     join o in this.workOrdersRepository.GetAll() on wt.WorkOrderId equals o.Id
                                     join m in this.machineRepository.GetAll() on wt.MachineId equals m.Id
                                     where wt.Id == input.Id
                                     select
                                         new MachineReportDto()
                                         {
                                             Id = wt.Id,
                                             WorkOrderId = o.Id,
                                             MachineId = wt.MachineId,
                                             MachineName = m.Name,
                                             ProductionPlanId = o.ProductionPlanId,
                                             ProductionPlanCode = o.ProductionPlan.Code,
                                             QualifiedCount = wt.QualifiedCount
                                         };
            var query = await machineReportQuery.FirstOrDefaultAsync();

            var workOrderReason =
                this.workOrderDefectiveRecordsRepository.GetAll().Where(w => w.WorkOrderTaskId == input.Id);
            var wordOrderReasonsDtoQuery = from d in this.defectiveRepository.GetAll()
                                           join wdr in workOrderReason on d.Id equals wdr.DefectiveReasonsId into g
                                           from k in g.DefaultIfEmpty()
                                           select
                                               new WordOrderReasonsDto()
                                               {
                                                   DefectiveReasonsId = d.Id,
                                                   ReasonName = d.Name,
                                                   Count = k.Count
                                               };
            query.ReasonsDictionary = await wordOrderReasonsDtoQuery.ToListAsync();

            return query;
        }

        /// <summary>
        /// 获取工单报工时默认的正品数和次品数= 每笔设备报工的汇总
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<WorkOrderReportDto> GetOutputQuantityForOrderReport(EntityDto input)
        {
            var returnValue = new WorkOrderReportDto();
            var query = this.workOrderTaskRepository.GetAll().Where(w => w.WorkOrderId == input.Id);
            if (!query.Any())
            {
                return returnValue;
            }

            returnValue.DefectiveCount = await query.SumAsync(w => w.DefectiveCount);
            returnValue.QualifiedCount = await query.SumAsync(w => w.QualifiedCount);

            return returnValue;
        }

        /// <summary>
        /// 获取工单列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DatatablesPagedResultOutput<WorkOrderDto>> ListWorkOrders(WorkOrdersRequestDto input)
        {
            var query = from w in this.workOrdersRepository.GetAll()
                        join p in this.productRepository.GetAll() on w.ProductionPlan.ProductId equals p.Id
                        join c in this.craftRepository.GetAll() on w.ProductionPlan.CraftId equals c.Id
                        join pp in this.craftProcessRepository.GetAll() on
                            new { w.ProductionPlan.CraftId, w.ProcessId } equals
                            new { pp.CraftId, pp.ProcessId }
                        join st in this.standardTimeRepository.GetAll() on
                            new { ProductId = p.Id, w.ProcessId } equals
                            new { st.ProductId, st.ProcessId } into g
                        from k in g.DefaultIfEmpty()
                        select
                            new WorkOrderDto()
                            {
                                Id = w.Id,
                                State = w.State,
                                StateName = w.State.ToString(),
                                Code = w.Code,
                                OrderCode = w.ProductionPlan.OrderCode,
                                ProductionPlanId = w.ProductionPlanId,
                                ProductionPlanCode = w.ProductionPlan.Code,
                                PlanState = w.ProductionPlan.ProductionPlanState.ToString(),
                                ProductCode = p.Code,
                                ProductName = p.Name,
                                CraftCode = c.Code,
                                CraftName = c.Name,
                                ProcessId = w.ProcessId,
                                ProcessCode = pp.ProcessCode,
                                ProcessName = pp.ProcessName,
                                ProcessOrderSeq = pp.ProcessOrder,
                                PutVolume = w.PutVolume,
                                AimVolume = w.AimVolume,
                                OutputCount = w.OutputCount,
                                QualifiedCount = w.QualifiedCount,
                                DefectiveCount = w.DefectiveCount,
                                IsLastProcessOrder = w.IsLastProcessOrder,
                                StandardTime = k.StandardCostTime,
                                CirculationRate = k.CycleRate,
                                CompletionRate = w.CompletionRate
                            };

            var totalcount = await query.CountAsync();
            query =
                query.WhereIf(
                    !string.IsNullOrEmpty(input.ProductionPlanCode),
                    q =>
                    q.ProductionPlanCode.Contains(input.ProductionPlanCode) || q.Code.Contains(input.ProductionPlanCode))
                    .WhereIf(
                        input.State.HasValue && input.State.Value != EnumWorkOrderState.All,
                        q => q.State == input.State.Value);

            var count = await query.CountAsync();
            var resutl = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new DatatablesPagedResultOutput<WorkOrderDto>(count, resutl, totalcount) { Draw = input.Draw };
        }

        /// <summary>
        /// 获取工单登录任务列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DatatablesPagedResultOutput<WorkOrderTaskDto>> ListWorkOrderTasks(
            WorkOrderTasksRequestDto input)
        {
            var query = from w in this.workOrderTaskRepository.GetAll()
                        join o in this.workOrdersRepository.GetAll() on w.WorkOrderId equals o.Id
                        join m in this.machineRepository.GetAll() on w.MachineId equals m.Id
                        join u in this.UserManager.Users on w.UserId equals u.Id
                        where w.WorkOrderId == input.Id
                        select
                            new WorkOrderTaskDto()
                            {
                                Id = w.Id,
                                WorkOrderId = w.WorkOrderId,
                                MachineId = w.MachineId,
                                MachineName = m.Name,
                                PutVolume = o.PutVolume,
                                AimVolume = o.AimVolume,
                                OutputCount = w.OutputCount,
                                QualifiedCount = w.QualifiedCount,
                                DefectiveCount = w.DefectiveCount,
                                StartTime = w.StartTime,
                                EndTime = w.EndTime,
                                UserId = w.UserId,
                                UserName = u.Name
                            };

            var count = await query.CountAsync();
            var result = await query.OrderByDescending(q => q.Id).PageBy(input).ToListAsync();

            return new DatatablesPagedResultOutput<WorkOrderTaskDto>(count, result, count);
        }

        /// <summary>
        /// 登录工单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Login(WorkOrderLoginDto input)
        {
            await this.workOrderManager.Login(input.Id, input.MachineIdList);
        }

        /// <summary>
        /// 设备报工
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task MachineReport(MachineReportDto input)
        {
            var reasonList =
                input.ReasonsDictionary.Select(
                    r => new NameValueDto<int>() { Name = r.DefectiveReasonsId.ToString(), Value = r.Count ?? 0 })
                    .ToList();

            await this.workOrderTaskManager.MachineReport(input.Id, input.QualifiedCount, reasonList);
        }

        /// <summary>
        /// 工单报工
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task WorkOrderReport(WorkOrderReportDto input)
        {
            try
            {
                var workOrder = await this.workOrdersRepository.GetAllIncluding(p => p.ProductionPlan).Where(p => p.Id == input.Id).FirstOrDefaultAsync();
                workOrder.Report(input.QualifiedCount, input.DefectiveCount);
            }
            catch (Exception)
            {
                throw new UserFriendlyException(this.L("WorkOrderFailure"));
            }
        }
    }
}
