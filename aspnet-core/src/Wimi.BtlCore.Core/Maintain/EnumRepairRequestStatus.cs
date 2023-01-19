using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore.Maintain
{
    public enum EnumRepairRequestStatus
    {
        /// <summary>
        /// 待维修
        /// </summary>
        Undo = 0,

        /// <summary>
        /// 维修中
        /// </summary>
        Overtime = 1,

        /// <summary>
        /// 已维修
        /// </summary>
        Done = 2,

    }
}
