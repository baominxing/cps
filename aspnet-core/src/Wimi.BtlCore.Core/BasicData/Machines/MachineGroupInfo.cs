using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.BasicData.Machines
{
    public class MachineGroupInfo
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        [Comment("设备名称")]
        public string MachineName { get; set; }

        /// <summary>
        /// 设备组Id
        /// </summary>
        [Comment("设备组Id")]
        public int GroupId { get; set; }

        /// <summary>
        /// 设备组名称
        /// </summary>
        [Comment("设备组名称")]
        public string GroupName { get; set; }

    }
}
