using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Order.Crafts;

namespace Wimi.BtlCore.Order.ProductionPlans
{


    public class AddWorkOrderManager : BtlCoreDomainServiceBase, IAddWorkOrderManager
    {
        private readonly IRepository<Craft> productCarftRepository;

        private readonly IRepository<WorkOrders.WorkOrder> workOrderRepository;

        public AddWorkOrderManager(
            IRepository<WorkOrders.WorkOrder> workOrderRepository,
            IRepository<Craft> productCarftRepository)
        {
            this.workOrderRepository = workOrderRepository;
            this.productCarftRepository = productCarftRepository;
        }

        public async Task CreateWorkOrder(int productCraftId, ProductionPlan productPlan)
        {
            var carft = await this.productCarftRepository.GetAllIncluding(x=>x.CraftProcesses).FirstAsync(x=>x.Id == productCraftId);

            var process = carft.CraftProcesses; // .ToList();

            var orders =
                process.Select(
                    c =>
                    new WorkOrders.WorkOrder()
                    {
                        Code = $"{productPlan.Code}-{c.ProcessOrder.ToString("0000")}",
                        AimVolume = productPlan.AimVolume,
                        ProcessId = c.ProcessId,
                        ProductionPlanId = productPlan.Id,
                        PutVolume = productPlan.PutVolume,
                        IsLastProcessOrder = c.IsLastProcess
                    });

            foreach (var workOrderse in orders)
            {
                workOrderse.NotStart();
                await this.workOrderRepository.InsertAsync(workOrderse);
            }
        }
    }
}
