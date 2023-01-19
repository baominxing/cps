using MongoDB.Bson;
using Newtonsoft.Json;
using System;

namespace Wimi.BtlCore.MongoEntities
{
    [Serializable]
    public class MongoEntity :IMongoEntity
    {
        [JsonIgnore]
        public ObjectId Id { get; set; }

        public int MachineId { get; set; }

        public string MachineCode { get; set; }

        public string ProgramName { get; set; }

        public int OrderId { get; set; }

        public int ProcessId { get; set; }

        public int ProductId { get; set; }

        public string PartNo { get;set;}

        public int MachinesShiftDetailId { get; set; }

        public long UserId { get; set; }

        public int UserShiftDetailId { get; set; }

        public int DateKey { get; set; }

        public string CreationTime { get; set; }
    }
}
