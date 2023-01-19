using Abp.Domain.Entities;
using MongoDB.Bson;
using Wimi.BtlCore.BTLMongoDB;

namespace Wimi.BtlCore.BasicData.Alarms.Mongo
{
    [MongoTable("Alarm")]
    public class MongoAlarm : Entity<ObjectId>
    {
        public int MachineId { get; set; }

        public string MachineCode { get; set; }

        public string ProgramName { get; set; }

        public int OrderId { get; set; }

        public int ProcessId { get; set; }

        public int ProductId { get; set; }

        public string PartNo { get; set; }

        public int MachinesShiftDetailId { get; set; }

        public long UserId { get; set; }

        public int UserShiftDetailId { get; set; }

        public int DateKey { get; set; }

        public string CreationTime { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public string ShiftSolutionName { get; set; }

        public string MachineShiftItemName { get; set; }

        public string StaffShiftItemName { get; set; }

        public string ShiftDay { get; set; }
    }
}