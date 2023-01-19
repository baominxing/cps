namespace Wimi.BtlCore.OEE
{
    using System.Collections.Generic;

    public class ShiftOeeResponse
    {
        public IEnumerable<OeeResponse> Availability { get; set; }

        public IEnumerable<OeeResponse> Performance { get; set; }

        public IEnumerable<OeeResponse> QualityIndicators { get; set; }
    }
}