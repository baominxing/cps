using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Localization;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;
using Wimi.BtlCore;
using Wimi.BtlCore.Trace;

namespace Wimi.BtlCore.Traceability.Dto
{
    [AutoMap(typeof(TraceFlowSetting))]
    public class TraceFlowSettingDto : ICustomValidate
    {
        public int Id { get; set; }

        public TraceFlowSettingDto()
        {
        }

        [Required]
        public int DeviceGroupId { get; set; }

        [Required]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Code { get; set; }

        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string DisplayName { get; set; }

        public int FlowSeq { get; set; }

        public int? PreFlowId { get; set; }

        public int? NextFlowId { get; set; }

        public FlowType FlowType { get; set; }

        public StationType StationType { get; set; }

        public bool WriteIntoPlc { get; set; }

        public bool WriteIntoPlcViaFlow { get; set; }

        public bool WriteIntoPlcViaFlowData { get; set; }

        public NameValueDto<int> ContentWriteIntoPlcViaFlow { get; set; }

        public NameValueDto<int> ContentWriteIntoPlcViaFlowData { get; set; }

        [Required]
        public TriggerEndFlowStyle TriggerEndFlowStyle { get; set; }

        public int? QualityMakerFlowId { get; set; }

        public OfflineByQuality OfflineByQuality { get; set; }

        public SourceOfPartNo SourceOfPartNo { get; set; }

        public bool NeedHandlerRelateData { get; set; }

        public RelateDataSourceSettings RelateDataSourceSettings { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (DeviceGroupId <= 0)
            {
                context.Results.Add(new ValidationResult(LocalizationHelper.GetString(BtlCoreConsts.LocalizationSourceName, "EffectiveProductionLinesMustBeEstablished")));
            }
        }
    }
}
