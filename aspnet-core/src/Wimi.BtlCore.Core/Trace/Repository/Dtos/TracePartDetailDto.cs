using Abp.Localization;
using Castle.Components.DictionaryAdapter;
using System;
using System.Collections.Generic;
using Wimi.BtlCore;


namespace Wimi.BtlCore.Trace.Repository.Dtos
{
    public class TracePartDetailDto
    {
        public TracePartDetailDto()
        {
            this.PartDetails = new PartDetail();
            this.TraceRecords = new EditableList<TraceFlowRecordDto>();
        }

        public PartDetail PartDetails { get; set; }

        public List<TraceFlowRecordDto> TraceRecords { get; set; }
    }

    public class PartDetail
    {
        public PartDetail()
        {
            Tags = new List<string>();
        }
        public string PartNo { get; set; }

        public List<string> Tags { get; set; }

        public string ShiftName { get; set; }

        public DateTime? OnlineTime { get; set; }

        public DateTime? OfflineTime { get; set; }

        public bool? Qualified { get; set; }

        public bool? IsReworkPart { get; set; }


        public void BuildTags()
        {
            if (Qualified.HasValue)
            {
                Tags.Add(Qualified.Value ? LocalizationHelper.GetString(BtlCoreConsts.LocalizationSourceName, "Qualified") : LocalizationHelper.GetString(BtlCoreConsts.LocalizationSourceName, "Unqualified"));
            }

            if (IsReworkPart.HasValue && IsReworkPart.Value)
            {
                Tags.Add(LocalizationHelper.GetString(BtlCoreConsts.LocalizationSourceName, "Rework"));
            }
        }
    }
}
