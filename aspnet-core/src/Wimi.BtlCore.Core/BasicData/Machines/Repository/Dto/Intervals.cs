using Castle.Components.DictionaryAdapter;
using System.Collections.Generic;

namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    public class Intervals
    {
        public Intervals()
        {
            this.State = new EditableList<DataParticles>();
            this.Reason = new EditableList<DataParticles>();
        }

        /// <summary>
        /// 甘特图时间范围
        /// </summary>
        public string[] Datetime { get; set; }

        /// <summary>
        /// 原因数据集合
        /// </summary>
        public List<DataParticles> Reason { get; set; }

        /// <summary>
        /// 状态数据集合
        /// </summary>
        public List<DataParticles> State { get; set; }
    }
}
