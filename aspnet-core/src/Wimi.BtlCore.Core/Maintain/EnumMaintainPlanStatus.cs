 

namespace Wimi.BtlCore.Maintain
{
    /// <summary>
    /// 保养计划状态
    /// </summary>
    public enum EnumMaintainPlanStatus
    {
        /// <summary>
        /// 新增
        /// </summary>
        New = 1,

        /// <summary>
        /// 进行中
        /// </summary>
        Processing = 2,

        /// <summary>
        /// 已完成
        /// </summary>
        Done = 4
    }

    /// <summary>
    /// 保养工单状态
    /// </summary>
    public enum EnumMaintainOrderStatus
    {
        Undo = 1, //未保养
        Done = 2, //已保养
        Over = 4  //已逾期
    }
}
