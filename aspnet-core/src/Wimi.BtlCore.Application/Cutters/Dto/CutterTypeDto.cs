namespace Wimi.BtlCore.Cutter.Dto
{
    using Abp.AutoMapper;
    using Abp.Domain.Entities.Auditing;

    [AutoMap(typeof(CutterType))]
    public class CutterTypeDto : FullAuditedEntity<int>
    {
        public string Code { get; set; }

        public string CreatorName { get; set; }

        public string CutterNoPrefix { get; set; }

        public bool IsCutterNoPrefixCanEdit { get; set; }

        public string LastModifierName { get; set; }

        public int MemberCount { get; set; }

        public string Name { get; set; }

        public int? PId { get; set; }
    }
}