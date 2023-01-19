using System.Collections.Generic;

namespace Wimi.BtlCore.Cutter.Dto
{
    public class SaveMachineCutterRatesInputDto
    {
        public SaveMachineCutterRatesInputDto()
        {
            this.MachineCutterRates = new List<MachineCutterRatesDto>();
        }

        public List<MachineCutterRatesDto> MachineCutterRates { get; set; }
    }

    public class MachineCutterRatesDto
    {
        public MachineCutterRatesDto()
        {
            CutterRates = new List<LoadingMachineCutterDetails>();
        }

        public string MachineName { get; set; }

        public List<LoadingMachineCutterDetails> CutterRates { get; set; }
    }
}
