using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Traceability.Dto
{
    public class NgPartsRequestDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int DeviceGroupId { get; set; }

        public string PartNo { get; set; }

        public List<int> MachineId { get; set; }

        public int DefectiveMachineId { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string StationCode { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "OnlineTime";
            }
        }
    }
}
