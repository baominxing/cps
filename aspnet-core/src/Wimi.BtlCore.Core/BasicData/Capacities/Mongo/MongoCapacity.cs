using Wimi.BtlCore.BTLMongoDB;
using Wimi.BtlCore.MongoEntities;

namespace Wimi.BtlCore.BasicData.Capacities.Mongo
{
    [MongoTable("Capacity")]
    public class MongoCapacity : MongoEntity
    {
        public int Yield { get; set; }

        public int CurrentProgramCount { get; set; }

        public int OriginalCount { get; set; }

        public int AccumulateCount { get; set; }

        public bool? IsQualified { get; set; }

        public int PlanId { get; set; }

        public string DmpId { get; set; }

        public string PreDmpId { get; set; }

        public string ShiftSolutionName { get; set; }

        public string MachineShiftItemName { get; set; }

        public string StaffShiftItemName { get; set; }

        public string ShiftDay { get; set; }

    }
}
