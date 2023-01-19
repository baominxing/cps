using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using Wimi.BtlCore;
using Wimi.BtlCore.Trace;

namespace Wimi.BtlCore.Trace.Repository.Dtos
{
    [AutoMapFrom(typeof(TraceFlowRecord))]

    public class TraceFlowRecordDto : EntityDto<long>
    {
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string PartNo { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength)]
        public string FlowCode { get; set; }

        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string FlowDisplayName { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength)]
        public string MachineCode { get; set; }

        public string MachineName { get; set; }

        public int MachineId { get; set; }

        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Station { get; set; }

        public DateTime EntryTime { get; set; }

        public DateTime? LeftTime { get; set; }

        public FlowState FlowState { get; set; }

        public FlowTag FlowTag { get; set; }

        public StationType StationType { get; set; }

        public bool ContainsExtensionData { get; set; }

        public string UserName { get; set; }

        public bool ShowFlowParameters
        {
            get
            {
                if (StationType == StationType.Machining)
                {
                    return true;
                }

                return ContainsExtensionData;
            }
        }

        public string FlowDataSource
        {
            get
            {
                if (StationType == StationType.Machining)
                {
                    return "mongo";
                }
                return "extensionData";
            }
        }
    }
}
