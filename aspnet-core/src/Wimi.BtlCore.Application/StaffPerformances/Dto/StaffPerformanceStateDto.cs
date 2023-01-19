namespace Wimi.BtlCore.StaffPerformances.Dto
{
    using Abp.AutoMapper;

    using Wimi.BtlCore.StaffPerformance;

    [AutoMap(typeof(StaffPerformanceState))]
    public class StaffPerformanceStateDto : StaffPerformanceState
    {
    }
}