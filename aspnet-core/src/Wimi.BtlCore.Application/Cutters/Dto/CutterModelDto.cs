namespace Wimi.BtlCore.Cutter.Dto
{
    using Abp.AutoMapper;
    using Abp.Domain.Entities.Auditing;
    using Wimi.BtlCore.Cutter;

    [AutoMap(typeof(CutterModel))]
    public class CutterModelDto : FullAuditedEntity<int>
    {
        public EnumCountingMethod CountingMethod { get; set; }

        public string CountingMethodDisplayText { get; set; }

        public string CreatorName { get; set; }

        public string CutterNoPrefix { get; set; }

        public int CutterTypeId { get; set; }

        public string LastModifierName { get; set; }
        public string ToolLifeCountingMethod { set; get; }
        public string Name { get; set; }

        public int OriginalLife { get; set; }

        public string Parameter1 { get; set; }

        public string Parameter10 { get; set; }

        public string Parameter2 { get; set; }

        public string Parameter3 { get; set; }

        public string Parameter4 { get; set; }

        public string Parameter5 { get; set; }

        public string Parameter6 { get; set; }

        public string Parameter7 { get; set; }

        public string Parameter8 { get; set; }

        public string Parameter9 { get; set; }

        public int WarningLife { get; set; }
    }
}