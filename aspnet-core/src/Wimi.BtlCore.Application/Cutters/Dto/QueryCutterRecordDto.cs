using Abp.Extensions;
using Abp.Runtime.Validation;
using Castle.Components.DictionaryAdapter;

namespace Wimi.BtlCore.Cutter.Dto
{
    using System;
    using System.Collections.Generic;

    using Wimi.BtlCore.Dto;

    /// <summary>
    /// 接受前端的查询装卸刀记录数据
    /// </summary> 
    public class QueryCutterRecordDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {

        public QueryCutterRecordDto()
        {
            this.MachineIdList = new EditableList<long>();
        }


        /// <summary>
        /// 刀具型号
        /// </summary>
        public int? CutterModelId { get; set; }

        /// <summary>
        /// 刀具编号
        /// </summary>
        public string CutterNo { get; set; }

        public DateTime? EndTime { get; set; }

        public List<long> MachineIdList { get; set; }

        public DateTime? StartTime { get; set; }

        public bool IsForExport { get; set; } = false;


        public void Normalize()
        {
            if (this.Sorting.IsNullOrWhiteSpace())
            {
                this.Sorting = " OperatorTime desc ";
            }
        }
    }
}