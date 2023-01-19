using Abp.Application.Services.Dto;

namespace Wimi.BtlCore.Order.LoginReports.Dtos
{
    /// <summary>
    /// 工单报功Dto
    /// </summary>
    public class WorkOrderReportDto : EntityDto
    {
        /// <summary>
        /// 次品数
        /// </summary>
        public int DefectiveCount { get; set; }

        /// <summary>
        /// 正品数
        /// </summary>
        public int QualifiedCount { get; set; }
    }
}
