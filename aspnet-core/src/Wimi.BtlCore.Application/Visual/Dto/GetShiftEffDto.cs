namespace Wimi.BtlCore.Visual.Dto
{
    using Castle.Components.DictionaryAdapter;
    using System.Collections.Generic;
    using Wimi.BtlCore.Common.Dtos;

    /// <summary>
    /// 看板班次利用率
    /// </summary>
    public class GetShiftEffDto
    {
        public GetShiftEffDto()
        {
            this.UtilizationRate = new EditableList<UtilizationRate>();
            this.ShiftEffSummarys = new List<ShiftEffSummary>();
        }

        public List<ShiftEffSummary> ShiftEffSummarys { get; set; }

        public List<UtilizationRate> UtilizationRate { get; set; }
    }
}