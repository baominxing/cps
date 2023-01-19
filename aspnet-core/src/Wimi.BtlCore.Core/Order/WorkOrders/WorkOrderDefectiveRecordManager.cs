using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;

namespace Wimi.BtlCore.Order.WorkOrders
{
    public class WorkOrderDefectiveRecordManager : BtlCoreDomainServiceBase
    {
        private readonly IRepository<WorkOrderDefectiveRecords> workOrderDefectiveRecordsRepository;

        public WorkOrderDefectiveRecordManager(
            IRepository<WorkOrderDefectiveRecords> workOrderDefectiveRecordsRepository)
        {
            this.workOrderDefectiveRecordsRepository = workOrderDefectiveRecordsRepository;
        }

        public async Task SaveOrUpdateDefectiveRecords(int taskId, List<NameValueDto<int>> reasonsList)
        {
            foreach (var item in reasonsList)
            {
                var defectiveReasonsId = int.Parse(item.Name);
                var taskReason =
                    this.workOrderDefectiveRecordsRepository.GetAll()
                        .FirstOrDefault(t => t.WorkOrderTaskId == taskId && t.DefectiveReasonsId == defectiveReasonsId);

                if (taskReason != null)
                {
                    taskReason.Count = item.Value;
                }
                else
                {
                    var dto = new WorkOrderDefectiveRecords()
                    {
                        DefectiveReasonsId = defectiveReasonsId,
                        Count = item.Value,
                        WorkOrderTaskId = taskId
                    };

                    await this.workOrderDefectiveRecordsRepository.InsertAsync(dto);
                }
            }
        }
    }
}
