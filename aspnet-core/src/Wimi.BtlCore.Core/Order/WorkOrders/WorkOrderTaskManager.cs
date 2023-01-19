using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.StaffPerformance;

namespace Wimi.BtlCore.Order.WorkOrders
{
    public class WorkOrderTaskManager : BtlCoreDomainServiceBase
    {
        private readonly MachineManager machineManager;

        private readonly IRepository<PerformancePersonnelOnDevice> performancePersonnelOnDeviceRepository;

        private readonly WorkOrderDefectiveRecordManager workOrderDefectiveRecordManager;

        private readonly IRepository<WorkOrderTasks> workOrderTaskRepository;

        public WorkOrderTaskManager(
            IRepository<WorkOrderTasks> workOrderTaskRepository,
            IRepository<PerformancePersonnelOnDevice> performancePersonnelOnDeviceRepository,
            MachineManager machineManager,
            WorkOrderDefectiveRecordManager workOrderDefectiveRecordManager)
        {
            this.workOrderTaskRepository = workOrderTaskRepository;
            this.performancePersonnelOnDeviceRepository = performancePersonnelOnDeviceRepository;
            this.machineManager = machineManager;
            this.workOrderDefectiveRecordManager = workOrderDefectiveRecordManager;
        }

        public async Task CreateWorkOrderTask(int workOrderId, List<int> machineIds)
        {
            foreach (var item in machineIds)
            {
                var task = new WorkOrderTasks()
                {
                    WorkOrderId = workOrderId,
                    MachineId = item,
                    UserId = await this.GetMachineUserId(item),
                    StartTime = DateTime.Now,
                    EndTime = null
                };

                await this.workOrderTaskRepository.InsertAsync(task);
            }
        }

        /// <summary>
        /// 设备报功: 对工单任务数据更新; 当有次品时，新增次品记录
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="qualifiedCount"></param>
        /// <param name="reasonsList"></param>
        /// <returns></returns>
        public async Task MachineReport(int taskId, int qualifiedCount, List<NameValueDto<int>> reasonsList)
        {
            var task = await this.workOrderTaskRepository.GetAsync(taskId);
            task.DefectiveCount = reasonsList.Sum(r => r.Value);
            task.QualifiedCount = qualifiedCount;
            task.OutputCount = task.DefectiveCount + task.QualifiedCount;

            await this.workOrderDefectiveRecordManager.SaveOrUpdateDefectiveRecords(task.Id, reasonsList);
        }

        public void UpdateWorkOrderTaskEndTime(int workOrderId)
        {
            var taskList =
                this.workOrderTaskRepository.GetAll().Where(w => w.WorkOrderId == workOrderId && w.EndTime == null);
            if (taskList.Any())
            {
                foreach (var item in taskList)
                {
                    item.EndTime = DateTime.Now;
                }
            }
        }

        private async Task<long> GetMachineUserId(int machineId)
        {
            var entity =
                await this.performancePersonnelOnDeviceRepository.FirstOrDefaultAsync(d => d.MachineId == machineId);
            if (entity != null)
            {
                return entity.UserId;
            }
            else
            {
                var machine = await this.machineManager.GetMachineByIdAsync(machineId);
                throw new UserFriendlyException(this.L("LogOnToTheDevice{0}", machine.Name));
            }
        }
    }
}
