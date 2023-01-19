using Abp.Localization;
using System;
using System.Collections.Generic;

namespace Wimi.BtlCore.Carton.CartonPrintings.Dtos
{
    public class GetCartonRecordOutputDto
    {
        public GetCartonRecordOutputDto()
        {
            Tags = new List<string>();
        }
        public string PartNo { get; set; }

        public List<string> Tags { get; set; }

        public DateTime? OnlineTime { get; set; }

        public DateTime? OfflineTime { get; set; }

        public bool? Qualified { get; set; }

        public bool? IsReworkPart { get; set; }

        public int DeviceGroupId { get; set; }

        public string DeviceGroupName { get; set; }


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
