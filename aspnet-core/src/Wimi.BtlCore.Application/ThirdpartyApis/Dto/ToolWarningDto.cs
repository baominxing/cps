using Abp.AutoMapper;
using Wimi.BtlCore.Cutter;

namespace Wimi.BtlCore.ThirdpartyApis.Dto
{
    [AutoMap(typeof(CutterStates))]
    public class ToolWarningDto
    {
        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public string CutterNo { get; set; }

        public int? CutterTValue { get; set; }

        public int OriginalLife { get; set; }

        public int RestLife { get; set; }

        public int UsedLife { get; set; }

        public int WarningLife { get; set; }

        public string CutterUsedStatus { get; set; }

        public string CutterLifeStatus { get; set; }

        public string CountingMethod { get; set; }
    }
}