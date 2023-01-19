namespace Wimi.BtlCore.Cutter.Dto
{
    using System.Collections.Generic;

    using Castle.Components.DictionaryAdapter;

    public class LoadOrUnLoadCuttersDto : CutterLoadAndUnloadRecordDto
    {
        public LoadOrUnLoadCuttersDto()
        {
            this.SelectedIds = new EditableList<int>();
        }

        public int CutterTVlaue { get; set; }

        public long? OperatorUserId { get; set; }

        /// <summary>
        /// 批量操作时选中的Id集合
        /// </summary>
        public List<int> SelectedIds { get; set; }
    }
}