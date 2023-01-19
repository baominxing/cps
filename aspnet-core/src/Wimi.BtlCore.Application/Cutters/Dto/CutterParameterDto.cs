namespace Wimi.BtlCore.Cutter.Dto
{
    using System.Collections.Generic;

    using Abp.AutoMapper;
    using Abp.Domain.Entities.Auditing;

    [AutoMap(typeof(CutterParameter))]
    public class CutterParameterDto : FullAuditedEntity<int>
    {
        public CutterParameterDto()
        {
            this.CutterParameterIds = new List<int>();
        }

        public string Code { get; set; }

        public string CreatorName { get; set; }

        /// <summary>
        /// 考虑为批量操作预留
        /// </summary>
        public List<int> CutterParameterIds { get; set; }

        public string LastModifierName { get; set; }

        public string Name { get; set; }
    }
}