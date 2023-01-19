using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wimi.BtlCore.BasicData.Machines.Repository.Dto
{
    /// <summary>
    /// 某一设备各程序号的产量
    /// </summary>
    public class Yield4PerProgramOutputDto
    {
        /// <summary>
        /// 平均生产节拍
        /// </summary>
        public decimal AvgDuration { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        public int MachineId { get; set; }

        /// <summary>
        /// 程序名
        /// </summary>
        public string ProgramName { get; set; }

        /// <summary>
        /// 产量
        /// </summary>
        public int Yield { get; set; }
    }
}
