using Abp.AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using Wimi.BtlCore.BasicData.Shifts;

namespace Wimi.BtlCore.BasicData.Machines
{
    [AutoMap(typeof(ShiftHistory))]
    public class MachineShiftDetail
    {
        [Comment("设备Id")]
        public int MachineId { get; set; }

        [Comment("设备班次Id")]
        public int MachineShiftDetailId { get; set; }

        [Comment("班次日")]
        public DateTime ShiftDay { get; set; }

        [Comment("工序编号")]
        public int RowNum { get; set; }

        [Comment("班次方案")]
        public int ShiftSolutionId { get; set; }

        [Comment("具体班次Id")]
        public int ShiftSolutionItemId { get; set; }

        [Comment("具体班次名称")]
        public string ShiftSolutionItemName { get; set; }
    }
}
