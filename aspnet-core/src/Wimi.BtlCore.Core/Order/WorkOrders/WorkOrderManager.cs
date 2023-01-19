using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Order.ProductionPlans;
using Wimi.BtlCore.Plan;

namespace Wimi.BtlCore.Order.WorkOrders
{
    /// <summary>
    /// 工单领域服务
    /// </summary>
    public class WorkOrderManager : BtlCoreDomainServiceBase, IDomainService
    {
        private readonly IRepository<WorkOrder> workOrdersRepository;

        private readonly WorkOrderTaskManager workOrderTaskManager;
        private readonly IRepository<ProductionPlan> productionPlanRepository;

        public WorkOrderManager(IRepository<WorkOrder> workOrdersRepository, WorkOrderTaskManager workOrderTaskManager, IRepository<ProductionPlan> productionPlanRepository)
        {
            this.workOrdersRepository = workOrdersRepository;
            this.workOrderTaskManager = workOrderTaskManager;
            this.productionPlanRepository = productionPlanRepository;
        }

        public async Task Close(int workOrderId)
        {
            var workOrder = await this.workOrdersRepository.GetAsync(workOrderId);
            workOrder.ProductionPlan = await productionPlanRepository.GetAsync(workOrder.ProductionPlanId);
            workOrder.Close();
            if (workOrder.IsLastProcessOrder)
            {
                this.CloseAllWorkOrderByPlanId(workOrder.ProductionPlanId);
            }
        }

        /// <summary>
        /// 工单登录:1、工单状态=&gt;生产中；2、处理上一笔工单任务的时间；3、创建工单任务；4、计划的第一笔工单登录，计划状态=&gt;进行
        /// </summary>
        /// <param name="workOrderId">
        /// The work Order Id.
        /// </param>
        /// <param name="machineIds">
        /// The machine Ids.
        /// </param>
        /// <returns>
        /// </returns>
        public async Task Login(int workOrderId, List<int> machineIds)
        {
            var order = await this.workOrdersRepository.GetAllIncluding(t=>t.ProductionPlan).FirstOrDefaultAsync(t=>t.Id == workOrderId);
            if(order == null)
            {
                throw new UserFriendlyException(this.L("NoMatchingData"));
            }
            order.Login();
            this.workOrderTaskManager.UpdateWorkOrderTaskEndTime(workOrderId);
            await this.workOrderTaskManager.CreateWorkOrderTask(workOrderId, machineIds);
        }

        private void CloseAllWorkOrderByPlanId(int planId)
        {
            var workOrderList =
                this.workOrdersRepository.GetAll()
                    .Where(wr => wr.ProductionPlanId == planId && wr.State != EnumWorkOrderState.Closed);

            if (!workOrderList.Any())
            {
                return;
            }

            foreach (var item in workOrderList)
            {
                item.Close();
            }
        }
    }
}
