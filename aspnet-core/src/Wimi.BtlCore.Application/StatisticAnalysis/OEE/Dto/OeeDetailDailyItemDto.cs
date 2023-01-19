namespace Wimi.BtlCore.StatisticAnalysis.OEE.Dto
{
    using System.Collections.Generic;
    using Wimi.BtlCore.BasicData.Machines.Repository.Dto;

    public class OeeDetailDailyItemDto
    {
        public OeeDetailDailyItemDto()
        {
            this.GanttChart = new GetMachineGanttChartOutputDto();
            this.ProcessingTimes = new List<ProcessingTimeDto>();
            this.QualityStatus = new List<QualityStatusDto>();
            this.ProductionStatus = new MachineAvailabilityDto();
        }

        public GetMachineGanttChartOutputDto GanttChart { get; set; }

        /// <summary>
        /// 生产状况
        /// </summary>
        public MachineAvailabilityDto ProductionStatus { get; set; }

        /// <summary>
        /// 质量情况
        /// </summary>
        public IEnumerable<QualityStatusDto> QualityStatus { get; set; }

        /// <summary>
        /// 加工节拍
        /// </summary>
        public IEnumerable<ProcessingTimeDto> ProcessingTimes { get; set; }
    }
}