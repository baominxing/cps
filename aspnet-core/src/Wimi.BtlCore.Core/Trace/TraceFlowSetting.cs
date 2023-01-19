using Abp;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wimi.BtlCore.Trace
{
    [Table("TraceFlowSettings")]
    public class TraceFlowSetting : FullAuditedEntity, IExtendableObject
    {
        public TraceFlowSetting()
        {
            RelatedMachines = new List<TraceRelatedMachine>();
            WriteIntoPlcViaFlow = false;
        }

        [Comment("设备组ID")]
        public int DeviceGroupId { get; set; }

        [Comment("流程编号")]
        [MaxLength(BtlCoreConsts.MaxLength)]
        public string Code { get; set; }

        [Comment("流程名称")]
        [MaxLength(BtlCoreConsts.MaxDescLength)]
        public string DisplayName { get; set; }

        [Comment("上一流程ID")]
        public int? PreFlowId { get; set; }

        [Comment("下一流程ID")]
        public int? NextFlowId { get; set; }

        [Comment("流程类别")]
        public FlowType FlowType { get; set; }

        [Comment("工位类型")]
        public StationType StationType { get; set; }

        [Comment("结束流程方式")]
        public TriggerEndFlowStyle TriggerEndFlowStyle { get; set; }

        [Comment("流程触发时写信号")]
        public bool WriteIntoPlcViaFlow { get; set; }

        [Comment("数据触发时写信号")]
        public bool WriteIntoPlcViaFlowData { get; set; }

        [Comment("流程顺序")]
        public int FlowSeq { get; set; }

        private NameValue<int> _contentWriteIntoPlcViaFlow { get; set; }

        [NotMapped]
        public NameValue<int> ContentWriteIntoPlcViaFlow
        {
            get
            {
                if (_contentWriteIntoPlcViaFlow == null)
                {
                    _contentWriteIntoPlcViaFlow = this.GetData<NameValue<int>>("ContentWriteIntoPlcViaFlow");
                }
                return _contentWriteIntoPlcViaFlow;
            }
            set
            {
                _contentWriteIntoPlcViaFlow = value;
                this.SetData("ContentWriteIntoPlcViaFlow", _contentWriteIntoPlcViaFlow);
            }
        }

        private NameValue<int> _contentWriteIntoPlcViaFlowData { get; set; }

        [NotMapped]
        public NameValue<int> ContentWriteIntoPlcViaFlowData
        {
            get
            {
                if (_contentWriteIntoPlcViaFlowData == null)
                {
                    _contentWriteIntoPlcViaFlowData = this.GetData<NameValue<int>>("ContentWriteIntoPlcViaFlowData");
                }
                return _contentWriteIntoPlcViaFlowData;
            }
            set
            {
                _contentWriteIntoPlcViaFlowData = value;
                this.SetData("ContentWriteIntoPlcViaFlowData", _contentWriteIntoPlcViaFlowData);
            }
        }

        /// <summary>
        /// 质量责任归属到特定流程
        /// </summary>
        [Comment("质量责任归属到特定流程")]
        public int? QualityMakerFlowId { get; set; }

        private OfflineByQuality _offlineByQuality;
        [NotMapped]
        public OfflineByQuality OfflineByQuality
        {
            get
            {
                if (_offlineByQuality == null)
                {
                    _offlineByQuality = this.GetData<OfflineByQuality>("OfflineByQuality");

                }
                return _offlineByQuality;
            }
            set
            {
                _offlineByQuality = value;
                this.SetData("OfflineByQuality", _offlineByQuality);
            }
        }

        [Comment("是否处理相关数据")] 
        public bool NeedHandlerRelateData { get; set; }

        [Comment("工件来源")] 
        public SourceOfPartNo SourceOfPartNo { get; set; }

        private RelateDataSourceSettings _relateDataSourceSetting;
        [NotMapped]
        public RelateDataSourceSettings RelateDataSourceSettings
        {
            get
            {
                if (_relateDataSourceSetting == null)
                {
                    _relateDataSourceSetting = this.GetData<RelateDataSourceSettings>("RelateDataSourceSettings");
                }
                return _relateDataSourceSetting;
            }
            set
            {
                _relateDataSourceSetting = value;
                this.SetData("RelateDataSourceSettings", _relateDataSourceSetting);
            }
        }

        [Comment("数据")]
        [MaxLength(500)]
        public string ExtensionData { get; set; }

        public virtual List<TraceRelatedMachine> RelatedMachines { get; set; }
    }

    public class RelateDataSourceSettings
    {
        public RelateDataSource Source { get; set; }

        public AuthorizationModel AuthModel { get; set; }
    }

    public class AuthorizationModel
    {
        public AuthorizationModel(string dataLocation, string authorisedAccount, string authorisedPassword)
        {
            DataLocation = dataLocation;
            AuthorisedAccount = authorisedAccount;
            AuthorisedPassword = authorisedPassword;
        }

        public string DataLocation { get; private set; }
        public string AuthorisedAccount { get; private set; }
        public string AuthorisedPassword { get; private set; }
    }

    public class OfflineByQuality
    {
        public bool OfflineWhenOk { get; set; }

        public bool OfflineWhenNg { get; set; }
    }

    public enum RelateDataSource
    {
        ByPlc,
        ByAccess,
        ByCsv,
        ByTxt
    }

    public enum FlowType
    {
        OnlineFlow,
        ProcessFlow,
        OfflineFlow,
        OptionFlow
    }

    public enum StationType
    {
        QualityInspection = 0,
        Machining = 1,
        Feeding = 2,
        FinalInspection = 4,
        Other = 3
    }

    public enum TriggerEndFlowStyle
    {
        SelfFlow = 1,
        PrevFlow = 2,
        SelfAndPrevFlow = 3
    }

    public enum SourceOfPartNo
    {
        ViaPlcVariable = 1,
        None = 2
    }
}
