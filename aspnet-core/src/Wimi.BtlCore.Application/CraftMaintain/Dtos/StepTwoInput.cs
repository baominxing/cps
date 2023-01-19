using System.Collections.Generic;

namespace Wimi.BtlCore.CraftMaintain.Dtos
{
    public class StepTwoInput
    {
        /// <summary>
        /// 工艺Id
        /// </summary>
        public int? CraftId { get; set; }
        /// <summary>
        /// 选择的工序Id集合
        /// </summary>
        public List<int> CraftProcesseIds { get; set; }
    }
}
