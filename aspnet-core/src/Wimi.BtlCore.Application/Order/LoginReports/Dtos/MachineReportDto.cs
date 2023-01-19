using System.Collections.Generic;
using System.Linq;

using Abp.Domain.Entities;

using Castle.Components.DictionaryAdapter;

namespace Wimi.BtlCore.Order.LoginReports.Dtos
{
    /// <summary>
    /// 设备报功Dto
    /// </summary>
    public class MachineReportDto : Entity
    {
        public MachineReportDto()
        {
            this.ReasonsDictionary = new EditableList<WordOrderReasonsDto>();
        }

        /// <summary>
        /// 次品数
        /// </summary>
        public int DefectiveCount
        {
            get
            {
                return this.ReasonsDictionary.Sum(r => r.Count ?? 0);
            }
        }

        /// <summary>
        /// 设备Id
        /// </summary>
        public int MachineId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// 生成计划
        /// </summary>
        public string ProductionPlanCode { get; set; }

        /// <summary>
        /// 计划Id
        /// </summary>
        public int ProductionPlanId { get; set; }

        /// <summary>
        /// 正品数
        /// </summary>
        public int QualifiedCount { get; set; }

        /// <summary>
        /// 次品原因Id，数量
        /// </summary>
        public List<WordOrderReasonsDto> ReasonsDictionary { get; set; }

        /// <summary>
        /// 工单id
        /// </summary>
        public int WorkOrderId { get; set; }
    }
}
