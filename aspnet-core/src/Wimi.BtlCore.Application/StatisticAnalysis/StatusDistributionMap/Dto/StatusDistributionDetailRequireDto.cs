using System;
using System.Collections.Generic;
using Abp.Extensions;
using Abp.Runtime.Validation;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.StatisticAnalysis.StatusDistributionMap.Dto
{
    public class StatusDistributionDetailRequireDto: PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int MachineId { get; set; }

        public DateTime ShiftDay { get; set; }

        public int? MachineShiftDetailId { get; set; }

        public List<string> UnionTables { get; internal set; } = new List<string>();

        public void Normalize()
        {
            if (this.Sorting.IsNullOrWhiteSpace())
            {
                this.Sorting = "StartTime ";
            }
        }
    }
}