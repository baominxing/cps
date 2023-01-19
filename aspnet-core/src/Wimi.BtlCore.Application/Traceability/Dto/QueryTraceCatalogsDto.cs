using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Traceability.Dto
{
    public class QueryTraceCatalogsDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<int> MachineId { get; set; }

        public int DefectiveMachineId { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public string StationCode { get; set; }

        public int? Id { get; set; }

        public string PartNo { get; set; }

        public int? DeviceGroupId { get; set; }

        public DateTime? StartFirstTime { get; set; }

        public DateTime? StartLastTime { get; set; }

        public DateTime? EndFirstTime { get; set; }

        public DateTime? EndLastTime { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public long NgPartCatlogId { get; set; }

        public string ArchivedTable { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "OnlineTime desc";
            }
        }
    }
}
