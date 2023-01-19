using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wimi.BtlCore.RDLCReport.Dto
{
    public class PersonYieldDto
    {
        /// <summary>
        /// 人员名称
        /// </summary>
        public string PersonName { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// 产量
        /// </summary>
        public decimal Yield { get; set; }
        /// <summary>
        /// 平均节拍=班次时长/产量
        /// </summary>
        public string AverageRhythm { get; set; }
        public DateTime ShiftDate { get; set; }
        public float Duration { get; set; }
        public int UserId { get; set; }
        public string ShiftName { get; set; }
        public string SolutionName { get; set; }
        public string UserName { get; set; }

    }
}
