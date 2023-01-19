using System.Collections.Generic;

namespace Wimi.BtlCore.RealtimeIndicators.Parameters.Dto
{
    public class AlarmPagesInputDto
    {
        /// <summary>
        /// 查询组列表
        /// </summary>
        public List<int> GroupIdList { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string MachineCode { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        public int MachineId { get; set; }

        /// <summary>
        /// 当前获取第几条数据
        /// </summary>
        public int PageNo { get; set; }

        /// <summary>
        /// 每次加载条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 分页
        /// </summary>
        public int CurrentPageNo { get; set; }


        public int CurrentPageSize { get; set; }
    }
}
