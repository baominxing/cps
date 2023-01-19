namespace Wimi.BtlCore.Cutter.Dto
{
    using System.Collections.Generic;

    using Abp.Runtime.Validation;

    using Castle.Components.DictionaryAdapter;
    using Wimi.BtlCore.Cutter;
    using Wimi.BtlCore.Dto;

    public class QueryCutterStateDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public QueryCutterStateDto()
        {
            this.CutterTypeIds = new EditableList<int>();
            this.CutterModelIds = new EditableList<int>();
            this.CutterUsedStateses = new EditableList<EnumCutterUsedStates>();
            this.CutterLifeStateses = new EditableList<EnumCutterLifeStates>();
        }

        /// <summary>
        /// 已选寿命状态集合
        /// </summary>
        public List<EnumCutterLifeStates> CutterLifeStateses { get; set; }

        /// <summary>
        /// 已选刀具型号集合
        /// </summary>
        public List<int> CutterModelIds { get; set; }

        /// <summary>
        /// 刀具编号
        /// </summary>
        public string CutterNo { get; set; }

        /// <summary>
        /// 刀位
        /// </summary>
        public decimal? CutterTValue { get; set; }

        /// <summary>
        /// 已选刀具类型集合
        /// </summary>
        public List<int> CutterTypeIds { get; set; }

        /// <summary>
        /// 已选使用状态集合
        /// </summary>
        public List<EnumCutterUsedStates> CutterUsedStateses { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string MachineNo { get; set; }

        public bool IsForExport { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "CutterLifeStatus DESC";
            }
        }
    }
}