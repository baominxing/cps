using System.Collections.Generic;

namespace Wimi.BtlCore.Carton.CartonPrintings.Dtos
{
    public class FinalInspecInputDto
    {
        public FinalInspecInputDto()
        {
            DefectivePartReasonIds = new List<int>();
        }

        public string PartNo { get; set; }

        public bool Qualified { get; set; }

        public List<int> DefectivePartReasonIds { get; set; }
    }
}
