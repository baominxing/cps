namespace Wimi.BtlCore.StaffPerformances.Dto
{
    using System.Collections.Generic;

    public class ProductionChartResultDto
    {
        public IEnumerable<UserMachinShiftYieldDto> UserMachinShiftYields { get; set; }
    }
}