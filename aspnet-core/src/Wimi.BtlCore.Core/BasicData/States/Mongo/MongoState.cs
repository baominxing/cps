using Wimi.BtlCore.BTLMongoDB;
using Wimi.BtlCore.MongoEntities;

namespace Wimi.BtlCore.States.Mongo
{
    [MongoTable("State")]
    public class MongoState:MongoEntity
    {
        public string Code { get; set; }

        public string DmpId { get; set; }

        public string PreDmpId { get; set; }

        public string ShiftSolutionName { get; set; }

        public string MachineShiftItemName { get; set; }

        public string StaffShiftItemName { get; set; }

        public string ShiftDay { get; set; }
    }
}
