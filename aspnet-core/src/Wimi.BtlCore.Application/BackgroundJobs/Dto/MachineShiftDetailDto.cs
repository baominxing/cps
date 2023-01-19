using System;
using Wimi.BtlCore.BasicData.Shifts;
using static Wimi.BtlCore.Machines.Mongo.MongoMachine;

namespace Wimi.BtlCore.BackgroundJobs.Dto
{
    public class MachineShiftDetailDto
    {
        public MachineShiftDetailDto()
        {

        }

        public MachineShiftDetailDto(int machineId, int previousMachinesShiftDetailId, int dateKey)
        {
            this.MachineId = machineId;
            this.MachineShiftDetailId = 0;
            this.ShiftDay = null;
            this.ShiftSolutionId = 0;
            this.ShiftSolutionItemId = 0;
            this.DateKey = dateKey;
            this.BeginTime = string.Empty;
            this.PreviousMachinesShiftDetailId = previousMachinesShiftDetailId;
            this.ShiftExtras = new MachineShiftExtras();
            this.ShiftDefail = new ShiftDefailInfo();
        }

        public int MachineId { get; set; }

        public int MachineShiftDetailId { get; set; }
        
        public DateTime? ShiftDay { get; set; }

        public int RowNum { get; set; }

        public int ShiftSolutionId { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public string ShiftSolutionItemName { get; set; }

        public string BeginTime { get; set; }

        public int DateKey { get; set; }

        public int PreviousMachinesShiftDetailId { get; set; }

        public MachineShiftExtras ShiftExtras { get; set; }

        public ShiftDefailInfo ShiftDefail { get; set; }
    }
}