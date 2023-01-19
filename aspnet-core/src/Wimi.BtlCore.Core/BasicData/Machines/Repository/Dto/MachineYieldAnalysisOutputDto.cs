using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;

namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class MachineYieldAnalysisOutputDto
    {
        public MachineYieldAnalysisOutputDto()
        {
            this.States = new EditableList<MachineStates4YieldAnalysisDto>();
        }

        /// <summary>
        /// 设备组Id
        /// </summary>
        public int MachineGroupId { get; set; }

        /// <summary>
        /// 设备组名称
        /// </summary>
        public string MachineGroupName { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        public long MachineId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// 设备当天状态信息
        /// </summary>
        public List<MachineStates4YieldAnalysisDto> States { get; set; }

        /// <summary>
        /// 设备利用率
        /// </summary>
        public decimal UtilizationRate { get; set; }

        /// <summary>
        /// 设备利用率 变化趋势
        /// </summary>
        public string UtilizationTendency { get; set; }

        /// <summary>
        /// 产量
        /// </summary>
        public int Yield { get; set; }
    }
}
