using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;

namespace Wimi.BtlCore.Order.MachineDefectiveRecords
{
    public class MachineDefectiveRecordManager : BtlCoreDomainServiceBase, IMachineDefectiveRecordManager
    {
        private readonly IRepository<MachineDefectiveRecord> machineDefectiveRecordRepository;

        public MachineDefectiveRecordManager(IRepository<MachineDefectiveRecord> machineDefectiveRecordRepository)
        {
            this.machineDefectiveRecordRepository = machineDefectiveRecordRepository;
        }

        public async Task CreateOrUpdateDefectiveRecords(int machineId, int shiftSolutionItemId, int productid, IEnumerable<NameValueDto<int>> reasons, DateTime startTime)
        {
            foreach (var item in reasons)
            {
                var reasonsId = Convert.ToInt32(item.Name);
                var endTime = startTime.AddDays(1);

                var reason = this.machineDefectiveRecordRepository.GetAll().FirstOrDefault(
                    t => t.MachineId == machineId && t.ShiftSolutionItemId == shiftSolutionItemId && t.ProductId == productid && t.DefectiveReasonsId == reasonsId
                         && t.CreationTime >= startTime && t.CreationTime < endTime);

                if (reason != null)
                {
                    reason.Count = item.Value;
                    reason.ShiftDay = startTime;
                }
                else
                {
                    await this.machineDefectiveRecordRepository.InsertAsync(
                        new MachineDefectiveRecord()
                        {
                            MachineId = machineId,
                            ShiftSolutionItemId = shiftSolutionItemId,
                            Count = item.Value,
                            DefectiveReasonsId = reasonsId,
                            ShiftDay = startTime,
                            ProductId = productid
                        });
                }
            }
        }
    }
}