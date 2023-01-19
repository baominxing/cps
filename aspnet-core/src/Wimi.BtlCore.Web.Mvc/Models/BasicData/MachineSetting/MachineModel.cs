using System.Collections.Generic;

namespace Wimi.BtlCore.Web.Models.BasicData.MachineSetting
{
    public class MachineModel
    {
        public MachineModel()
        {
            this.MachineShiftSolution = new List<MachineShiftSolutionViewModel>();
        }

        public int Id { get; set; }

        public List<MachineShiftSolutionViewModel> MachineShiftSolution { get; set; }
    }
}
