using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Shifts.Manager.Dto;
using Wimi.BtlCore.CommonEnums;

namespace Wimi.BtlCore.BasicData.Shifts.Manager
{
    public interface IShiftCalendarManager : IDomainService
    {
        /// <summary>
        /// 获取设备列表关联的班次方案列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<GetMachineShiftSolutionsDto>> GetMachineShiftSolutions(DateTime startTime, DateTime endTime, List<int> machineIdLList);

        /// <summary>
        /// 获取设备组列表关联的班次方案列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<GetMachineShiftSolutionsDto>> GetMachineGroupShiftSolutions(DateTime startTime, DateTime endTime, List<int> machineGroupIdList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="statisticalWay"></param>
        /// <returns></returns>
        Task<IEnumerable<GetSummaryDateDto>> GetSummaryDate(DateTime startTime, DateTime endTime, EnumStatisticalWays statisticalWay);

        /// <summary>
        /// 根据传入时间范围，计算按周，按月，按年统计需要的正确的开始时间和结束时间
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="statisticalWay"></param>
        /// <param name="machineIdList"></param>
        /// <param name="shiftSolutionIdList"></param>
        /// <returns></returns>
        Task<IEnumerable<CorrectQueryDateDto>> CorrectQueryDate(DateTime startTime, DateTime endTime, EnumStatisticalWays statisticalWay, List<int> machineIdList, List<int> shiftSolutionIdList);
    }
}
