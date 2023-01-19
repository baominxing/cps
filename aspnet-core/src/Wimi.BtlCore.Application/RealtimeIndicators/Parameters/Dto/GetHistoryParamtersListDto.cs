namespace Wimi.BtlCore.RealtimeIndicators.Parameters.Dto
{
    using MongoDB.Bson;
    using System.Collections.Generic;
    using Wimi.BtlCore.BasicData.Machines;

    public class GetHistoryParamtersListDto
    {
        public List<DataTablesColumns> ParamColumns { get; set; }

        public List<Dictionary<string, string>> ParamData { get; set; }

        public string MachineName { get; set; }
    }
    public class GetHistoryParamtersListExportDto
    {
        public IEnumerable<MachineGatherParam> ParamList { get; set; }

        public List<Dictionary<string, string>> ParamData { get; set; }

        public IEnumerable<BsonDocument> Parameters { get; set; }

        public string MachineName { get; set; }
    }
}