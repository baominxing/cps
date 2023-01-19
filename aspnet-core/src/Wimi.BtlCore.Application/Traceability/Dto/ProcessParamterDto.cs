using MongoDB.Bson;
using System.Collections.Generic;
using Wimi.BtlCore.BasicData.Dto;

namespace Wimi.BtlCore.Traceability.Dto
{
    public class ProcessParamterDto
    {
        public IEnumerable<IEnumerable<BsonElement>> ParamList { get; set; }

        public IEnumerable<MachineGatherParamDto> MachineGatherList { get; set; }
    }

}
