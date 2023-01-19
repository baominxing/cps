
using System.Collections.Generic;

namespace Wimi.BtlCore.Cutter.Dto
{
    public class ListLoadingMachineCuttersDto
    {
        public string MachineName { get; set; }

        public string MachineCode { get; set; }

        public int MachineId { get; set; }

        public List<LoadingMachineCutterDetails> Details { get; set; }

        public string Active { get; set; }
    }

    public class LoadingMachineCutterDetails
    {
        public int TValue { get; set; }

        public int Rate { get; set; }
    }
}
