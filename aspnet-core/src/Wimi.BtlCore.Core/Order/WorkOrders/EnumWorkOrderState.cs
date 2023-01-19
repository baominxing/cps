using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.Order.WorkOrders
{
    /// <summary>
    /// 工单状态
    /// </summary>
    public enum EnumWorkOrderState
    {
        /// <summary>
        /// The all.
        /// </summary>
        [Display(Name = "全部")]
        All = 0,

        /// <summary>
        /// 未开始
        /// </summary>
        [Display(Name = "未开始")]
        NotStart = 1,

        /// <summary>
        /// 生产中
        /// </summary>
        [Display(Name = "生产中")]
        Producing = 2,

        /// <summary>
        /// 关闭
        /// </summary>
        [Display(Name = "关闭")]
        Closed = 4
    }
}
