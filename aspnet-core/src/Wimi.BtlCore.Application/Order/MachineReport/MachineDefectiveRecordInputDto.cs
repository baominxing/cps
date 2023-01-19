using System;
using System.Collections.Generic;

using Abp.Application.Services.Dto;

namespace Wimi.BtlCore.Order.MachineReport
{
    public class MachineDefectiveRecordInputDto
    {
        public MachineDefectiveRecordInputDto()
        {
            this.Reasons = new List<NameValueDto<int>>();
        }

        public IEnumerable<NameValueDto<int>> Reasons { get; set; }

        public int MachineId { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public DateTime Date { get; set; }

        public int ProductId { get; set; }
    }
}
