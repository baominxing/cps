using System;

namespace WIMI.BTL.Machines.RepositoryDto.State
{
    public class ListMahcineStateMapDto
    {
        public int Id { get; set; }

        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public int MachineSeq { get; set; }

        public int DateKey { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string StateName { get; set; }

        public string Hexcode { get; set; }

        public string Code { get; set; }

        public string StateCode { get; set; }

        public string ShiftName { get; set; }

        public DateTime RStartTime { get; set; }

        public DateTime REndTime { get; set; }

        public int ShiftSolutionItemId { get; set; }

        public int MachinesShiftDetailId { get; set; }

        /// <summary>
        /// NONE，OUT，LEFT，RIGHT，IN
        /// </summary>
        public string STATETYPE { get; set; }
    }
}
