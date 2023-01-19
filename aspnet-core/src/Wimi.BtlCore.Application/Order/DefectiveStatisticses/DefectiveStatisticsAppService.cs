using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.Order.DefectiveReasons;
using Wimi.BtlCore.Order.DefectiveStatisticses.Dtos;
using Wimi.BtlCore.Order.Products;
using Wimi.BtlCore.Order.Products.Dtos;
using Wimi.BtlCore.Order.WorkOrders;

namespace Wimi.BtlCore.Order.DefectiveStatisticses
{
    public class DefectiveStatisticsAppService : BtlCoreAppServiceBase, IDefectiveStatisticsAppService
    {
        private readonly IRepository<DefectiveReason> defectiveReasonRepository;

        private readonly IRepository<Machine> machineRepository;

        private readonly IRepository<Products.Product> productRepository;

        private readonly IRepository<WorkOrderDefectiveRecords> workOrderDefectiveRecordRepository;

        private readonly IRepository<WorkOrder> workOrderRepository;

        private readonly IRepository<WorkOrderTasks> workOrderTaskRepository;

        public DefectiveStatisticsAppService(
            IRepository<Machine> machineRepository,
            IRepository<WorkOrderDefectiveRecords> workOrderDefectiveRecordRepository,
            IRepository<WorkOrderTasks> workOrderTaskRepository,
            IRepository<DefectiveReason> defectiveReasonRepository,
            IRepository<WorkOrder> workOrderRepository,
            IRepository<Products.Product> productRepository)
        {
            this.machineRepository = machineRepository;
            this.workOrderDefectiveRecordRepository = workOrderDefectiveRecordRepository;
            this.workOrderTaskRepository = workOrderTaskRepository;
            this.defectiveReasonRepository = defectiveReasonRepository;
            this.workOrderRepository = workOrderRepository;
            this.productRepository = productRepository;
        }

        public async Task<IEnumerable<DefectiveStatisticsDto>> DefectiveStatisticsList(
            DefectiveStatisticRequestDto input)
        {
            var query = from wr in this.workOrderDefectiveRecordRepository.GetAll()
                        join wt in this.workOrderTaskRepository.GetAll() on wr.WorkOrderTaskId equals wt.Id
                        join w in this.workOrderRepository.GetAll() on wt.WorkOrderId equals w.Id
                        join dr in this.defectiveReasonRepository.GetAll() on wr.DefectiveReasonsId equals dr.Id
                        join m in this.machineRepository.GetAll() on wt.MachineId equals m.Id
                        join u in this.UserManager.Users on wt.UserId equals u.Id
                        select
                            new DefectiveStatisticsDto
                            {
                                ProductId = w.ProductionPlan.ProductId,
                                ProductionPlanCode = w.ProductionPlan.Code,
                                ProductCode = w.ProductionPlan.Product.Code,
                                ProductName = w.ProductionPlan.Product.Name,
                                WorkOrderCode = w.Code,
                                MachineCode = m.Code,
                                MachineName = m.Name,
                                DefectiveReasonCode = dr.Code,
                                DefectiveReasonName = dr.Name,
                                Count = wr.Count,
                                UserId = wt.UserId,
                                UserName =u.Name,
                                CreationTime = wr.CreationTime
                            };

            var endTime = DateTime.Now;
            if (input.EndTime != null)
            {
                endTime = input.EndTime.Value.AddDays(1).Date;
            }

            query =
                query.WhereIf(
                    input.ProductId.HasValue && input.ProductId.Value > 0,
                    q => q.ProductId == input.ProductId)
                    .WhereIf(
                        input.StartTime.HasValue && input.EndTime.HasValue,
                        q => q.CreationTime >= input.StartTime.Value && q.CreationTime < endTime);

            return await query.OrderBy(n => n.ProductId).ThenBy(n => n.CreationTime).ToListAsync();
        }

        public async Task<IEnumerable<ProductDto>> FindProductList()
        {
            var list = await this.productRepository.GetAllListAsync();
            return ObjectMapper.Map<IEnumerable<ProductDto>>(list);
        }
    }
}
