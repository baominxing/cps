using System.Collections.Generic;
using Wimi.BtlCore.BTLMongoDB;
using Wimi.BtlCore.MongoEntities;

namespace Wimi.BtlCore.Machines.Mongo
{
    [MongoTable("Machine")]
    public class MongoMachine : MongoEntity
    {

        public string Name { get; set; }

        public bool IsActive { get; set; }

        //[BsonIgnore]
        public string LastModificationTime { get; set; }

        public ICollection<MachineAlarm> Alarm { get; set; } = new List<MachineAlarm>();

        public MachineShiftExtras ShiftExtras { get; set; } = new MachineShiftExtras();

        public MachineState State { get; set; } = new MachineState();

        public MachineCapacity Capacity { get; set; } = new MachineCapacity();

        public IDictionary<string, object> Parameter { get; set; } = new Dictionary<string, object>();


        public class MachineAlarm
        {
            public string Code { get; set; }

            public string CreationTime { get; set; }

            public string Message { get; set; }
        }

        public class MachineState
        {
            public string DmpId { get; set; }
            public string Code { get; set; }

            public string CreationTime { get; set; }
        }

        public class MachineCapacity
        {
            public string DmpId { get; set; }

            public int Yield { get; set; }

            public int CurrentProgramCount { get; set; }

            public int OriginalCount { get; set; }

            public int AccumulateCount { get; set; }

            public string CreationTime { get; set; }
        }


        public class MachineShiftExtras
        {
            public string ShiftDay { get; set; }

            public string ShiftSolutionName { get; set; }

            public string MachineShiftItemName { get; set; }

            public string StaffShiftItemName { get; set; }
        }

    }
}
