using MongoDB.Bson;
using System.Collections.Generic;

namespace Wimi.BtlCore.Parameters.Dto
{
    public class GetHistoryParamtersDataTableDto
    {
        public GetHistoryParamtersDataTableDto()
        {
            this.ParamData = new Dictionary<string, string>();
        }

        public BsonObjectId ObjectId { get; set; }

        public Dictionary<string, string> ParamData { get; set; }
    }
}
