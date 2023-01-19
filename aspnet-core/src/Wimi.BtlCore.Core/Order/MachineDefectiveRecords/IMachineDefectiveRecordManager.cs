using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.Domain.Services;

namespace Wimi.BtlCore.Order.MachineDefectiveRecords
{
    public interface IMachineDefectiveRecordManager : IDomainService
    {
        Task CreateOrUpdateDefectiveRecords(int machineId, int shiftSolutionItemId, int productid, IEnumerable<NameValueDto<int>> reasons, DateTime startTime);
    }
}
