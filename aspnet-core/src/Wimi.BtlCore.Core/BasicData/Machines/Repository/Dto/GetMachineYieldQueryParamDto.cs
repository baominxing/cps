using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;

namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class GetMachineYieldQueryParamDto
    {
        public GetMachineYieldQueryParamDto()
        {
            this.DataList = new EditableList<string>();
            this.MachineIdList = new EditableList<int>();
        }

        /// <summary>
        /// 日期
        /// </summary>
        public List<string> DataList { get; set; }

        /// <summary>
        /// 设备Id列表
        /// </summary>
        public List<int> MachineIdList { get; set; }

        public int TotalCount { get; set; }
    }
}
