using System.ComponentModel.DataAnnotations;

namespace Wimi.BtlCore.BasicData.Machines
{
    public enum EnumMachineState
    {
        /// <summary>
        /// 停机
        /// </summary>
        Stop = 1,

        /// <summary>
        /// 运行
        /// </summary>
        Run = 2,

        /// <summary>
        /// 空闲
        /// </summary>
        Free = 3,

        /// <summary>
        /// 离线
        /// </summary>
        Offline = 4,

        /// <summary>
        /// 调试
        /// </summary>
        Debug = 5
    }

    public enum EnumMachineStateType
    {
        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        State = 0,

        /// <summary>
        /// 原因
        /// </summary>
        [Display(Name = "原因")]
        Reason = 1
    }

    public enum EnumMachineStatePlan
    {
        /// <summary>
        /// 无计划
        /// </summary>
        NonePlan = 1,

        /// <summary>
        /// 计划内
        /// </summary>
        InPlan = 2,

        /// <summary>
        /// 计划外
        /// </summary>
        OutPlan = 3
    }
}
