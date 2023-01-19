namespace Wimi.BtlCore.StaffPerformances.Dto
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class MongoMachineDto
    {
        public ObjectId Id { get; set; }

        public bool IsActive { get; set; }

        public int MachineId { get; set; }

        public string MachineCode { get; set; }

        public int MachinesShiftDetailId { get; set; }

        public string Name { get; set; }

        public int OrderId { get; set; }

        public string PartNo { get; set; }

        public int ProcessId { get; set; }

        public string ProgramName { get; set; }

        public int UserId { get; set; }

        public int UserShiftDetailId { get; set; }
    }
}
