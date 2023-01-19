using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IExtendableObject = Wimi.BtlCore.Extensions.IExtendableObject;


namespace Wimi.BtlCore.Trace
{
    [Table("TraceFlowRecords")]
    public class TraceFlowRecord : Entity<long>, IExtendableObject
    {
        [Comment("工件编号")]
        [MaxLength(BtlCoreConsts.MaxLength*2)]
        //[Index]
        public string PartNo { get; set; }

        [Comment("流程编号")]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string FlowCode { get; set; }

        [Comment("流程名称")]
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string FlowDisplayName { get; set; }

        [Comment("流程设定ID")]
        public int TraceFlowSettingId { get; set; }

        [Comment("设备编号")]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string MachineCode { get; set; }

        [Comment("设备ID")]
        public int MachineId { get; set; }

        [Comment("工位")]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Station { get; set; }

        [Comment("进入时间")]
        public DateTime EntryTime { get; set; }

        [Comment("离开时间")]
        public DateTime? LeftTime { get; set; }

        /// <summary>
        /// 流程状态：进行中，完成
        /// </summary>
        [Comment("流程状态")]
        public FlowState State { get; set; }

        /// <summary>
        /// 流程标识：合格，不合格
        /// </summary>
        [Comment("流程标识")]
        public FlowTag Tag { get; set; }

        [Comment("数据")]
        public string ExtensionData { get; set; }

        [Comment("用户ID")]
        public long UserId { get; set; }

        [Comment("归档表")]
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string ArchivedTable { get; set; }

    }

    public enum FlowState
    {
        Wip, // work in process
        Done
    }

    public enum FlowTag
    {
        Unknow = 0,
        Qualified = 1,
        UnQualified = 2
    }
}
