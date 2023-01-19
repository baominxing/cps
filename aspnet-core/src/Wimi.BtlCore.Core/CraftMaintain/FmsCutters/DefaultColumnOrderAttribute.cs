using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore.FmsCutters
{
    public class DefaultColumnOrderAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public int Order { get; set; }

        public DefaultColumnOrderAttribute(int order)
        {
            this.Order = order;
        }

        /// <summary>
        /// 初始默认属性
        /// </summary>
        /// <param name="order">顺序</param>
        /// <param name="displayName">替代展示字段，如MachineName 代替MachineId</param>
        public DefaultColumnOrderAttribute(int order, string displayName)
        {
            this.Order = order;
            this.DisplayName = displayName;
        }
    }
}
